namespace api.Services;

public interface IVersionsStorage
{
    Task<FileStream>    GetVersionFile    (string versionId);
    
    Task<string?>       WriteVersionOnDisk(IFormFile formFile, string id, string tag);
    Task                DeleteVersionFile (string id);
}
