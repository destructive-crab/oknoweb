using api.Services;
using Microsoft.AspNetCore.Authentication;
using api.Services.Auth;
using api.Controllers;

public static class DDAPI
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 200 * 1024 * 1024);
        
        builder.Services.AddScoped<IConfig,          Config>();
        builder.Services.AddScoped<IDatabaseReader,  DatabaseController>();
        builder.Services.AddScoped<IDatabaseWriter,  DatabaseController>();
        builder.Services.AddScoped<IVersionsStorage, VersionsStorage>();
        
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
