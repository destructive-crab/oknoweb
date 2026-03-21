using System.Text.Json;

namespace api.Services;

public sealed class Config : IConfig
{
    public const string CONFIG_PATH = "/home/destructive_crab/dev/okno/web/oknoweb/ddapi/config";
    
    public string IDColumn          => "id";
    public string NameColumn        => "name";
    public string TagColumn         => "tag";
    public string PathColumn        => "path";
    public string ChangelogColumn   => "changelog";
    public string ReleaseDateColumn => "release_date";

    public string DatabasePath { get; private set; }

    public string ArchiveMainPath { get; private set; }
    public string ArchiveModsPath { get; private set; }

    private readonly string RootPath;

    public Config()
    {
        RootPath        = File.ReadAllText(CONFIG_PATH);
        RootPath = RootPath.Replace("\n", "");

        DatabasePath    = Path.Combine(RootPath, "database.db");
        ArchiveMainPath = Path.Combine(RootPath, "versions_archive", "main");
        ArchiveModsPath = Path.Combine(RootPath, "versions_archive", "mods");
        
        ValidatePath(RootPath);
        ValidatePath(DatabasePath);
        ValidatePath(ArchiveMainPath);
        ValidatePath(ArchiveModsPath);
    }

    private void ValidatePath(string path)
    {
        if (!Path.Exists(path))
        {
            Console.WriteLine($"Invalid path: {JsonSerializer.Serialize(path)}");
        }
    }
}