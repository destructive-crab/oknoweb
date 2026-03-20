using api.Services;

public static class DDAPI
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddScoped<IConfig,          Config>();
        builder.Services.AddScoped<IVersionsService, VersionsService>();
        
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(VersionsController).Assembly);

        WebApplication app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}
