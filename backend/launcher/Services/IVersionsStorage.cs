namespace api.Services;

public interface IVersionsStorage
{
    Task<FileStream> GetWindowsVersionFile(string versionId);
    Task<FileStream> GetLinuxVersionFile(string versionId);
    
    Task<string>        WriteVersionOnDisk(IFormFile formFile, string fileName, string tag);
    Task                DeleteVersionFile (string id);
}
