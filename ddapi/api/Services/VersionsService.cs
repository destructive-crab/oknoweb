using Microsoft.Data.Sqlite;

namespace api.Services;

public sealed class VersionsService : IVersionsService
{
    private readonly IConfig Config;
    
    public VersionsService(IConfig config)
    {
        Config = config;
    }
    
    public async Task<VersionInfo[]> GetVersions()
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");
        
        await connection.OpenAsync();

        using SqliteCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT * FROM main";
        
        using SqliteDataReader reader = command.ExecuteReader();

        List<VersionInfo> infos = new();
        
        while (await reader.ReadAsync())
        {
            infos.Add(new VersionInfo(reader["id"] as string, 
                                      reader["path"] as string,
                                      reader["changelog"] as string, 
                                      reader["release_date"] as string));
        }

        return infos.ToArray();
    }
    
    public async Task<VersionInfo> GetVersion(string id)
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");

        await connection.OpenAsync();

        using SqliteCommand command = new SqliteCommand($"SELECT * FROM main WHERE id = '{id}'", connection);
        
        using SqliteDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new VersionInfo(
            reader["id"] as string,
            reader["path"] as string,
            reader["changelog"] as string,
            reader["release_date"] as string);
    }
    
    public async Task<FileStream> GetVersionFile(string versionId)
    {
        VersionInfo info = await GetVersion(versionId);

        return new FileStream(info.Path, FileMode.Open, FileAccess.Read);
    }
}