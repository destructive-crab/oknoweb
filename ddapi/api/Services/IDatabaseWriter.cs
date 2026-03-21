namespace api.Services;

public interface IDatabaseWriter
{
    Task RegisterVersion      (VersionInfo info);
    Task EditVersion          (string id, VersionInfo info);
    Task DeleteVersion        (string id);
}