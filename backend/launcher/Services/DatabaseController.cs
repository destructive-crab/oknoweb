using api.Debug;
using Microsoft.Data.Sqlite;

namespace api.Services;

public sealed class DatabaseController : IDatabaseReader, IDatabaseWriter
{
    private readonly IConfig Config;
    private readonly ILocalLogger Logger;

    private LocalVersionInfo CreateInfoFromReader(SqliteDataReader reader)
    {
        LocalVersionInfo info =  new LocalVersionInfo(
            new(reader[Config.IDColumn]          as string,
                reader[Config.NameColumn]        as string,
                reader[Config.TagColumn]         as string,
                reader[Config.ChangelogColumn]   as string,
                reader[Config.ReleaseDateColumn] as string,
                Int32.Parse(reader[Config.DownloadsCount] as string)),
            reader[Config.WindowsPathColumn] as string,
            reader[Config.LinuxPathColumn]   as string
            );

        //hardcoded stuff
        //need to setup all api and server domain in config

        if (info.WindowsZipPath != "none")
        {
            info.PublicInfo.DownloadWindowsURL = $"https://oknoweb.ru/api/versions/files/windows/{info.PublicInfo.ID}";
        }

        if (info.LinuxZipPath != "none")
        {
            info.PublicInfo.DownloadLinuxURL   = $"https://oknoweb.ru/api/versions/files/linux/{info.PublicInfo.ID}";
            
        }
        return info;
    }

    public DatabaseController(IConfig config, ILocalLogger logger)
    {
        Config = config;
        Logger = logger;
    }

    public async Task<bool> ValidateUserAuth(string username, string password)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();
            await using (SqliteCommand command = new SqliteCommand($"SELECT * FROM users WHERE username = '{username}' and password = '{password}'", connection))
	        {
                await using (SqliteDataReader reader = await command.ExecuteReaderAsync())
		        {
		            return await reader.ReadAsync();
		        }
	        }	
	    }	
    }

    public async Task<bool> HasVersion(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand($"SELECT EXISTS(SELECT 1 FROM main WHERE id = '{id}')", connection))
            {
                object? result = await command.ExecuteScalarAsync();
                int count = Convert.ToInt32(result);

                return count == 1;
            }
        }
    }

    public async Task<LocalVersionInfo?> ReadVersionInfo(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand($"SELECT * FROM main WHERE id = '{id}'", connection))
            {
                await using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    bool valid = await reader.ReadAsync();

                    if (valid)
                    {
                        return CreateInfoFromReader(reader);    
                    }
                    else
                    {
                        return null;
                    }
                }   
            }
        }
    }

    public async Task<LocalVersionInfo[]> ReadAllVersionsInfo()
    {
        List<LocalVersionInfo> infos = new();

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

    public async Task RegisterVersion(LocalVersionInfo info)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string insertCommand = $"insert into main ({Config.IDColumn}, {Config.WindowsPathColumn}, {Config.WindowsPathColumn}, {Config.NameColumn}, {Config.TagColumn}, {Config.ChangelogColumn}, {Config.ReleaseDateColumn}) values(@id, @win_path, @linux_path, @name, @tag, @changelog, @release_date)";

            await using (SqliteCommand command = new(insertCommand, connection))
            {
                command.Parameters.AddWithValue("@id",           info.PublicInfo.ID);
                command.Parameters.AddWithValue("@name",         info.PublicInfo.Name);
                command.Parameters.AddWithValue("@win_path",     info.WindowsZipPath);
                command.Parameters.AddWithValue("@linux_path",   info.LinuxZipPath);
                command.Parameters.AddWithValue("@tag",          info.PublicInfo.Tag);
                command.Parameters.AddWithValue("@changelog",    info.PublicInfo.Changelog);
                command.Parameters.AddWithValue("@release_date", info.PublicInfo.ReleaseDate);

                await command.ExecuteNonQueryAsync();        
            }
        }
    }

    public async Task IncreaseDownloadsCount(string versionID)
    {
        LocalVersionInfo? info = await ReadVersionInfo(versionID);

        if (info == null)
        {
            Logger.Error($"Failed getting version with ID: {versionID} while trying to increase downloads count.");
            
            return;
        }
        
        int newCount = info.PublicInfo.Downloads + 1;
        await WriteDownloadsCount(versionID, newCount);
    }
    
    public async Task WriteDownloadsCount(string versionID, int newCount)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.DownloadsCount} = @downloads_count " +
                                 $"where {Config.IDColumn} = '{versionID}'";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.Add("@downloads_count", SqliteType.Text).Value = newCount.ToString();

                await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    public async Task WriteVersion(string versionID, LocalVersionInfo info)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.IDColumn} = @id, {Config.WindowsPathColumn} = @win_path, {Config.LinuxPathColumn} = @linux_path, {Config.NameColumn} = @name, {Config.TagColumn} = @tag, {Config.ChangelogColumn} = @changelog, {Config.ReleaseDateColumn} = @release_date " +
                                 $"where {Config.IDColumn} = '{versionID}'";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.Add("@id",           SqliteType.Text).Value = info.PublicInfo.ID;
                command.Parameters.Add("@win_path",     SqliteType.Text).Value = info.WindowsZipPath;
                command.Parameters.Add("@linux_path",   SqliteType.Text).Value = info.LinuxZipPath;
                command.Parameters.Add("@name",         SqliteType.Text).Value = info.PublicInfo.Name;
                command.Parameters.Add("@tag",          SqliteType.Text).Value = info.PublicInfo.Tag;
                command.Parameters.Add("@changelog",    SqliteType.Text).Value = info.PublicInfo.Changelog;
                command.Parameters.Add("@release_date", SqliteType.Text).Value = info.PublicInfo.ReleaseDate;

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