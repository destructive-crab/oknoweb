namespace api.Services;

public interface IVersionsService
{
    public Task<VersionInfo[]> GetVersions();
    public Task<VersionInfo> GetVersion(string id);
}

[Serializable]
public sealed class VersionInfo
{
    public string ID { get; set; }
    public string ReleaseDate { get; set; }
    public string Path { get; set; }
    public string Changelog { get; set; }

    public VersionInfo(string id, string releaseDate, string path, string changelog)
    {
        ID = id;
        ReleaseDate = releaseDate;
        Path = path;
        Changelog = changelog;
    }

    public override string ToString()
    {
        return $"{ID} {Path}";
    }
}