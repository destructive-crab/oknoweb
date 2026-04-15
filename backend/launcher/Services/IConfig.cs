namespace api.Services;

public interface IConfig
{
    public string IDColumn          { get; }
    public string WindowsPathColumn { get; }
    public string LinuxPathColumn   { get; }
    public string NameColumn        { get; }
    public string TagColumn         { get; }
    public string ChangelogColumn   { get; }
    public string ReleaseDateColumn { get; }
    public string DownloadsCount    { get; }
    
    public string DatabasePath       { get; }
    public string VersionArchivePath { get; }
}