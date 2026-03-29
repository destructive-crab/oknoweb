using System.Reflection;
using System.Text.Json;
using api.Debug;

namespace api.Services;

public sealed class Config : IConfig
{
    public string CONFIG_PATH => Path.Combine(Utils.GetConfigDirectory(), "config");
    
    public string IDColumn          => "id";
    public string NameColumn        => "name";
    public string TagColumn         => "tag";
    public string PathColumn        => "path";
    public string ChangelogColumn   => "changelog";
    public string ReleaseDateColumn => "release_date";

    public string DatabasePath       { get; private set; }
    public string VersionArchivePath { get; private set; }

    private readonly string RootPath;

    public Config(ILocalLogger logger)
    {
        RootPath        = File.ReadAllText(CONFIG_PATH);
        RootPath        = RootPath.Replace("\n", "");

        DatabasePath       = Path.Combine(RootPath, "database.db");
        VersionArchivePath = Path.Combine(RootPath, "versions_archive");
        
        ValidatePath(RootPath, logger);
        ValidatePath(DatabasePath, logger);
    }

    private void ValidatePath(string path, ILocalLogger logger)
    {
        if (!Path.Exists(path))
        {
            logger.Error($"Invalid path: {JsonSerializer.Serialize(path)}");
        }
    }
}