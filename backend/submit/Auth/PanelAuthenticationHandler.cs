using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Auth;

public class PanelAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfig Config;
    
    public PanelAuthenticationHandler(
	    IConfig config,
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
        Config = config;
    }

    private async Task<bool> Validate(string username, string password)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.UsersDatabasePath}"))
        {
            await connection.OpenAsync();
            await using (SqliteCommand command = new SqliteCommand($"SELECT * FROM users WHERE username = '{username}' and password = '{password}'", connection))
	        {
                await using (SqliteDataReader reader = await command.ExecuteReaderAsync())
		        {
		            return await reader.ReadAsync();
		        }
	        }	
	    }	
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
			Console.WriteLine("No authorization header");
            return AuthenticateResult.Fail("No authorization header");
        }

        string authorizationHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authorizationHeader))
        {
			Console.WriteLine("Invalid header. Empty");
            return AuthenticateResult.Fail("Invalid header. Empty");
        }

        if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
			Console.WriteLine("Invalid header. Basic auth required");
            return AuthenticateResult.Fail("Invalid header. Basic auth required");
        }

        var token = authorizationHeader.Substring(6);
        var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

        var credentials = credentialAsString.Split(":");
        if (credentials?.Length != 2)
        {
			Console.WriteLine("Invalid arguments in username:password");
            return AuthenticateResult.Fail("Invalid arguments in username:password");
        }
        
        var username = credentials[0];
        var password = credentials[1];

		bool valid = await Validate(username, password);
	
		if(valid)
		{
		    var claims = new[]
		    {
				new Claim(ClaimTypes.NameIdentifier, username)
		    };
		    
		    var identity = new ClaimsIdentity(claims, "Basic");
		    var claimsPrincipal = new ClaimsPrincipal(identity);
		
		    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));	    
		}

		return AuthenticateResult.Fail("Invalid username or password");
    }
}
