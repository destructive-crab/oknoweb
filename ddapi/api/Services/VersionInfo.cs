namespace api.Services;

[Serializable]
public sealed class VersionInfo
{
    public string ID { get; set; }
    public string ReleaseDate { get; set; }
    public string Path { get; set; }
    public string Changelog { get; set; }

    public VersionInfo(string id, string path, string changelog, string releaseDate)
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