namespace api.Services;

public interface IConfig
{
    public string DatabasePath { get; }
    public string ArchiveMainPath { get; }
    public string ArchiveModsPath { get; }
    
}