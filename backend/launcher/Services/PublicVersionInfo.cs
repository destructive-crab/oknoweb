namespace api.Services;

[Serializable]
public sealed class LocalVersionInfo
{
    public PublicVersionInfo PublicInfo { get; private set; }
    public string            Path       { get; set; }

    public LocalVersionInfo(PublicVersionInfo publicInfo, string path)
    {
        PublicInfo = publicInfo;
        Path = path;
    }
}

[Serializable]
public sealed class PublicVersionInfo
{
    public string ID          { get; set; }
    
    public string Name        { get; set; }
    public string Tag         { get; set; }
    public string Changelog   { get; set; }
    public string ReleaseDate { get; set; }

    public PublicVersionInfo(string id, string name, string tag, string changelog, string releaseDate)
    {
        ID = id; 
        ReleaseDate = releaseDate;
        Name = name;
        Tag = tag;
        Changelog = changelog;
    }

    public override string ToString()
    {
        return $"{ID}: {Name} {Tag}";
    }
}