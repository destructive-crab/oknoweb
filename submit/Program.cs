using Microsoft.AspNetCore.Authentication;
using Debug;
using Auth;

namespace Submit;

public static class OknoSubmit
{
    public static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        
        builder.Services.AddSingleton<ILocalLogger,        LocalLogger>();
        
        builder.Services.AddSingleton<IConfig,             Config>();
        builder.Services.AddSingleton<IDatabaseReader,     DatabaseController>();
        builder.Services.AddSingleton<IDatabaseWriter,     DatabaseController>();

        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(SubmissionsController).Assembly);

		builder.Services.AddAuthentication("PanelAuthentication")
		    .AddScheme<AuthenticationSchemeOptions, PanelAuthenticationHandler>("PanelAuthentication", null);

		builder.Services.AddAuthorization();
		
        WebApplication app = builder.Build();

		app.UseAuthentication();
		app.UseAuthorization();
	
        app.MapControllers();
	
        app.Run();
    }
}
