using api.Services;
using Microsoft.AspNetCore.Authentication;
using api.Services.Auth;
using api.Controllers;
using api.Debug;

public static class DDAPI
{
    public static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        
        builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 200 * 1024 * 1024);
        
        builder.Services.AddSingleton<ILocalLogger,     LocalLogger>();
        
        builder.Services.AddSingleton<IConfig,          Config>();
        builder.Services.AddSingleton<IDatabaseReader,  DatabaseController>();
        builder.Services.AddSingleton<IDatabaseWriter,  DatabaseController>();
        builder.Services.AddSingleton<IVersionsStorage, VersionsStorage>();
        
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(VersionsController).Assembly);


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