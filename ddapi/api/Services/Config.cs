namespace api.Services;

public sealed class Config : IConfig
{
    public string DatabasePath { get; private set; }

    public const string CONFIG_PATH = "/home/destructive_crab/dev/okno/web/oknoweb/ddapi/config";
    
    public Config()
    {
        DatabasePath = File.ReadAllText(CONFIG_PATH);
    }
}