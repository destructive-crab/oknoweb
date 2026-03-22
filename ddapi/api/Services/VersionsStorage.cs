namespace api.Services;

public sealed class VersionsStorage : IVersionsStorage
{
    private readonly IConfig Config;

    public IDatabaseWriter Writer { get; set; }
    public IDatabaseReader Reader { get; set; }

    public VersionsStorage(IConfig config, IDatabaseReader reader, IDatabaseWriter writer)
    {
        Config = config;
        Reader = reader;
        Writer = writer;
    }

    public async Task<FileStream> GetVersionFile(string versionId)
    {
        VersionInfo info = await Reader.ReadVersionInfo(versionId);

        return new FileStream(info.Path, FileMode.Open, FileAccess.Read);
    }

    public async Task<string?> WriteVersionOnDisk(IFormFile formFile, string id, string tag)
    {
        try
        {
            string path = Path.Combine(Config.ArchiveMainPath, id + ".zip");
            
            using (var stream = File.Create(path))
            {
                await formFile.CopyToAsync(stream);
            }
            
            return path;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed writing file on disk: {0}", ex);
            return null;
        }
    }

    public async Task DeleteVersionFile(string id)
    {
	VersionInfo info = await Reader.ReadVersionInfo(id);

	if(info == null)
	{
	    return;
	}
	
	File.Delete(info.Path);		
    }
}
