namespace api.Services;

public interface IDatabaseWriter
{
    Task         RegisterVersion     (LocalVersionInfo info);
    Task         WriteVersion         (string id, LocalVersionInfo info);
    Task         DeleteVersion       (string id);
    Task IncreaseDownloadsCount      (string versionID);
}