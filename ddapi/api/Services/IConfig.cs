namespace api.Services;

public interface IConfig
{
    public string IDColumn          { get; }
    public string PathColumn        { get; }
    public string NameColumn        { get; }
    public string TagColumn         { get; }
    public string ChangelogColumn   { get; }
    public string ReleaseDateColumn { get; }
    
    public string DatabasePath      { get; }
    
    public string ArchiveMainPath   { get; }
    public string ArchiveModsPath   { get; }
}