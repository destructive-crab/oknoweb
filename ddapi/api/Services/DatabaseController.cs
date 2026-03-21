using Microsoft.Data.Sqlite;

namespace api.Services;

public sealed class DatabaseController : IDatabaseReader, IDatabaseWriter
{
    private readonly IConfig Config;

    private VersionInfo CreateInfoFromReader(SqliteDataReader reader)
    {
        return new VersionInfo(
            reader[Config.IDColumn]          as string,
            reader[Config.PathColumn]        as string,
            reader[Config.NameColumn]        as string,
            reader[Config.TagColumn]         as string,
            reader[Config.ChangelogColumn]   as string,
            reader[Config.ReleaseDateColumn] as string);
    }

    public DatabaseController(IConfig config)
    {
        Config = config;
    }

    public Task<bool> ValidateUserAuth(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<VersionInfo> ReadVersionInfo(string id)
    {
        using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            using (SqliteCommand command = new SqliteCommand($"SELECT * FROM main WHERE id = '{id}'", connection))
            {
                using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    bool valid = await reader.ReadAsync();

                    if (valid)
                    {
                        return CreateInfoFromReader(reader);    
                    }
                    else
                    {
                        throw new ArgumentException("Invalid ID");
                    }
                }   
            }
        }
    }

    public async Task<VersionInfo[]> ReadAllVersionsInfo()
    {
        List<VersionInfo> infos = new();

        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand("SELECT * FROM main", connection))
            {
                await using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        infos.Add(CreateInfoFromReader(reader));
                    }            
                }            
            }            
        }
        
        return infos.ToArray();
    }

    public async Task RegisterVersion(VersionInfo info)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string insertCommand = $"insert into main ({Config.IDColumn}, {Config.PathColumn}, {Config.NameColumn}, {Config.TagColumn}, {Config.ChangelogColumn}, {Config.ReleaseDateColumn}) values(@id, @path, @name, @tag, @changelog, @release_date)";

            await using (SqliteCommand command = new(insertCommand, connection))
            {
                command.Parameters.AddWithValue("@id", info.ID);
                command.Parameters.AddWithValue("@path", info.Path);
                command.Parameters.AddWithValue("@name", info.Name);
                command.Parameters.AddWithValue("@tag", info.Tag);
                command.Parameters.AddWithValue("@changelog", info.Changelog);
                command.Parameters.AddWithValue("@release_date", info.ReleaseDate);

                await command.ExecuteNonQueryAsync();        
            }
        }
    }

    public async Task EditVersion(string versionID, VersionInfo info)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.IDColumn} = @id, {Config.PathColumn} = @path, {Config.NameColumn} = @name, {Config.TagColumn} = @tag, {Config.ChangelogColumn} = @changelog, {Config.ReleaseDateColumn} = @release_date " +
                                 $"where {Config.IDColumn} = '{versionID}'";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.Add("@id",           SqliteType.Text).Value = info.ID;
                command.Parameters.Add("@path",         SqliteType.Text).Value = info.Path;
                command.Parameters.Add("@name",         SqliteType.Text).Value = info.Name;
                command.Parameters.Add("@tag",          SqliteType.Text).Value = info.Tag;
                command.Parameters.Add("@changelog",    SqliteType.Text).Value = info.Changelog;
                command.Parameters.Add("@release_date", SqliteType.Text).Value = info.ReleaseDate;

                Console.WriteLine(command.CommandText);
                Console.WriteLine(" ");
                
                await command.ExecuteNonQueryAsync();                
            }
        }
    }
    
    public async Task DeleteVersion(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new($"delete from main where {Config.IDColumn} = '{id}'", connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}