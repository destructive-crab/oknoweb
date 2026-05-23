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
            reader[Config.DateColumn]           as string
        );
    }

    public DatabaseController(IConfig config, ILocalLogger logger)
    {
        Config = config;
        Logger = logger;
    }

    public async Task<bool> HasSubmission(string id)
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

    public async Task<PrivateSubmit?> Read(string id)
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
                                 $"where {Config.IDColumn} = '{info.ID}'";

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

            await using (SqliteCommand command = new($"delete from main where {Config.IDColumn} = '{id}'", connection))
            {
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
                                 $"set {Config.StatusColumn} = '{PublicSubmit.PENDING_STATUS}'" +
                                 $"where {Config.IDColumn} = '{subid}'";

            await using (SqliteCommand command = new(editCommand, connection))
            {
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
                                 $"set {Config.StatusColumn} = '{PublicSubmit.REVIEWED_STATUS}', {Config.ReviewLinkColumn} = '{link}'" +
                                 $"where {Config.IDColumn} = '{subid}'";

            await using (SqliteCommand command = new(editCommand, connection))
            {
                await command.ExecuteNonQueryAsync();                
            }
        }
    }
}
