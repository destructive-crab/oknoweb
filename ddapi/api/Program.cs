using api.Services;

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

        WebApplication app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}
