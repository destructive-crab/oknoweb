namespace api.Services;

[Serializable]
public sealed class VersionInfo
{
    public string ID          { get; set; }
    public string Path        { get; set; }
    
    public string Name        { get; set; }
    public string Tag         { get; set; }
    public string Changelog   { get; set; }
    public string ReleaseDate { get; set; }

    public VersionInfo(string id, string path, string name, string tag, string changelog, string releaseDate)
    {
        ID = id; 
        ReleaseDate = releaseDate;
        Name = name;
        Tag = tag;
        Path = path;
        Changelog = changelog;
    }

    public override string ToString()
    {
        return $"{ID} {Name} {Path}";
    }
}