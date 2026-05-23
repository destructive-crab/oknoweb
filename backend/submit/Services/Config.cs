using System.Text.Json;
using Debug;

namespace Submit;

public sealed class Config : IConfig
{
    public string IDColumn             => "id"; 
    public string StatusColumn         => "status";
    public string NameColumn           => "name";
    public string LinkColumn           => "link";
    public string DateColumn           => "date"; 
    public string ContactColumn        => "contact";
    public string AdditionalInfoColumn => "additionalInfo";
    public string ReviewLinkColumn     => "videoLink";

    public string CONFIG_PATH => Path.Combine(Utils.GetConfigDirectory(), "config");

    public string UsersDatabasePath { get; private set; }
    public string DatabasePath      { get; private set; }

    private readonly string RootPath;

    public Config(ILocalLogger logger)
    {
        RootPath        = File.ReadAllText(CONFIG_PATH);
        RootPath        = RootPath.Replace("\n", "");

        DatabasePath      = Path.Combine(RootPath, "database.db");
        UsersDatabasePath = DatabasePath;       

        ValidatePath(RootPath, logger);
        ValidatePath(DatabasePath, logger);
        ValidatePath(UsersDatabasePath, logger);
    }

    private void ValidatePath(string path, ILocalLogger logger)
    {
        if (!Path.Exists(path))
        {
            logger.Error($"Invalid path: {JsonSerializer.Serialize(path)}");
        }
    }
}
