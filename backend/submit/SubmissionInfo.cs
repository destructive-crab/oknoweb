using Debug;

namespace Submit;

[Serializable]
public sealed class PrivateSubmit : PublicSubmit
{
    public string Contact { get; set; }

    public PrivateSubmit(string contact, string id, string status, string name, string link, string additionalInfo, string date)
        : base(id, status, name, link, additionalInfo, date)
    {
        Contact = contact.Trim();
    }

}

[Serializable]
public class PublicSubmit 
{
    public static async Task<string> GenerateID(IDatabaseReader reader, ILocalLogger logger)
    {
        try
        {
            int maxID = 0;
            PrivateSubmit[] all = await reader.ReadAll();
            
            if (all.Length > 0)
            {
                maxID = Int32.Parse(all[all.Length - 1].ID);
            }

            return (maxID + 1).ToString();
        }
        catch (Exception e)
        {
            logger.Error($"Failed generating ID {e.ToString()}");
            return "invalid";
        }
    }
    
    public const string REVIEWED_STATUS  = "reviewed";
    public const string UNVERIFIED_STATUS  = "unverified";
    public const string PENDING_STATUS = "pending";

    public string ID { get; set; } = "invalid";
    public string Status { get; set; } = "invalid";

    public string Name { get; set; } = "invalid";
    public string Link { get; set; } = "invalid";
    public string AdditionalInfo { get; set; } = "invalid";
    public string ReviewLink { get; set; } = "none"; 
    public string Date { get; set; } = "invalid";

    public PublicSubmit(string id, string status, string name, string link, string additionalInfo, string date)
    {
        ID = id.Trim();
        Name = name.Trim();
        Status = status.Trim();
        Link = link.Trim();
        AdditionalInfo = additionalInfo.Trim();
        Date = date.Trim();
    }
}
