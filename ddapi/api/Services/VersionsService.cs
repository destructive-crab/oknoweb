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
        using SqliteConnection connection = new SqliteConnection($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/dev/test.db");
        
        await connection.OpenAsync();

        using SqliteCommand command = connection.CreateCommand();
        
        command.CommandText = $"SELECT * FROM main WHERE id = {id}";
        
        using SqliteDataReader reader = command.ExecuteReader();

        return new VersionInfo(reader["id"] as string,
            reader["path"] as string,
            reader["changelog"] as string,
            reader["release_date"] as string);
    }

}