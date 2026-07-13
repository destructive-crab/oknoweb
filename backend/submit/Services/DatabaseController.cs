using Debug;
using Microsoft.Data.Sqlite;

namespace Submit;

public sealed class DatabaseController : IDatabaseReader, IDatabaseWriter
{
    private readonly IConfig Config;
    private readonly ILocalLogger Logger;

    private PrivateSubmit CreateInfoFromReader(SqliteDataReader reader)
    {
        return new(
            reader[Config.ContactColumn]        as string,

            reader[Config.IDColumn]             as string,
            reader[Config.StatusColumn]         as string,
            reader[Config.NameColumn]           as string,
            reader[Config.LinkColumn]           as string,
            reader[Config.AdditionalInfoColumn] as string,
            reader[Config.DateColumn]           as string,
            reader[Config.ReviewLinkColumn]     as string
        );
    }

    public DatabaseController(IConfig config, ILocalLogger logger)
    {
        Config = config;
        Logger = logger;
    }
    
    public async Task<bool> ValidateUser(string username, string password)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.UsersDatabasePath}"))
        {
            await connection.OpenAsync();
            await using (SqliteCommand command = new SqliteCommand("SELECT * FROM users WHERE username = @username AND password = @password", connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                await using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    return await reader.ReadAsync();
                }
            }
        }
    }
    public async Task<bool> HasSubmission(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand("SELECT EXISTS(SELECT 1 FROM main WHERE id = @id)", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                object? result = await command.ExecuteScalarAsync();
                int count = Convert.ToInt32(result);

                return count == 1;
            }
        }
    }

    public async Task<PrivateSubmit?> Read(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand("SELECT * FROM main WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
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

    public async Task<PrivateSubmit[]> ReadAll()
    {
        List<PrivateSubmit> infos = new();

        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new SqliteCommand("SELECT * FROM main", connection))
            {
                await using (SqliteDataReader reader = await command.ExecuteReaderAsync())
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

    public async Task WriteSubmission(PrivateSubmit info)
    {
        if (!await HasSubmission(info.ID))
        {
            await AddSubmission(info);
            return;
        }
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.IDColumn} = @id, {Config.StatusColumn} = @status, {Config.NameColumn} = @name, {Config.ContactColumn} = @contact, {Config.LinkColumn} = @link, {Config.AdditionalInfoColumn} = @additionalInfo, {Config.DateColumn} = @date, {Config.ReviewLinkColumn} = @reviewLink" + 
                                 $"where {Config.IDColumn} = @where_id";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.Add("@id",             SqliteType.Text).Value = info.ID;
                command.Parameters.Add("@status",         SqliteType.Text).Value = info.Status;
                command.Parameters.Add("@contact",        SqliteType.Text).Value = info.Contact;
                command.Parameters.Add("@name",           SqliteType.Text).Value = info.Name;
                command.Parameters.Add("@link",           SqliteType.Text).Value = info.Link;
                command.Parameters.Add("@additionalInfo", SqliteType.Text).Value = info.AdditionalInfo;
                command.Parameters.Add("@date",           SqliteType.Text).Value = info.Date;
                command.Parameters.Add("@reviewLink",      SqliteType.Text).Value = info.ReviewLink;
                command.Parameters.Add("@where_id",        SqliteType.Text).Value = info.ID;

                await command.ExecuteNonQueryAsync();                
            }
        }
    }

    private async Task AddSubmission(PrivateSubmit info)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string insertCommand = $"insert into main " +
                                   $"({Config.IDColumn}, {Config.StatusColumn}, {Config.ContactColumn}, " +
                                   $"{Config.NameColumn}, {Config.LinkColumn}, {Config.AdditionalInfoColumn}, " +
                                   $"{Config.DateColumn}, {Config.ReviewLinkColumn}) " +
                                   $"values(@id, @status, @contact, @name, @link, @additionalInfo, @date," +
                                   $"'https://www.youtube.com/watch?v=dQw4w9WgXcQ&pp=ygUJcmljayByb2xs')";

            await using (SqliteCommand command = new(insertCommand, connection))
            {
                command.Parameters.Add("@id",             SqliteType.Text).Value = info.ID;
                command.Parameters.Add("@status",         SqliteType.Text).Value = info.Status;
                command.Parameters.Add("@name",           SqliteType.Text).Value = info.Name;
                command.Parameters.Add("@contact",        SqliteType.Text).Value = info.Contact;
                command.Parameters.Add("@link",           SqliteType.Text).Value = info.Link;
                command.Parameters.Add("@additionalInfo", SqliteType.Text).Value = info.AdditionalInfo;
                command.Parameters.Add("@date",           SqliteType.Text).Value = info.Date;

                await command.ExecuteNonQueryAsync();                
            }
        }
    }

    public async Task DeleteSubmission(string id)
    {
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            await using (SqliteCommand command = new("DELETE FROM main WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task MarkAsPending(string subid)
    {
        if (!await HasSubmission(subid)) return;
        
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.StatusColumn} = @status " +
                                 $"where {Config.IDColumn} = @subid";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.AddWithValue("@status", PublicSubmit.PENDING_STATUS);
                command.Parameters.AddWithValue("@subid",  subid);
                await command.ExecuteNonQueryAsync();                
            }
        }
    }

    public async Task MarkAsReviewed(string subid, string link)
    {
        if (!await HasSubmission(subid)) return;
        
        await using (SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}"))
        {
            await connection.OpenAsync();

            string editCommand = $"update main " +
                                 $"set {Config.StatusColumn} = @status, {Config.ReviewLinkColumn} = @link " +
                                 $"where {Config.IDColumn} = @subid";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                command.Parameters.AddWithValue("@status", PublicSubmit.REVIEWED_STATUS);
                command.Parameters.AddWithValue("@link",   link);
                command.Parameters.AddWithValue("@subid",  subid);
                await command.ExecuteNonQueryAsync();                
            }
        }
    }
}
