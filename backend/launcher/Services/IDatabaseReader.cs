namespace api.Services;

public interface IDatabaseReader
{
    Task<bool>                ValidateUserAuth    (string username, string password);
    Task<bool>                HasVersion          (string id);
    Task<LocalVersionInfo?>    ReadVersionInfo     (string id);
    Task<LocalVersionInfo[]>  ReadAllVersionsInfo ();
}