namespace Submit;

public interface IDatabaseWriter
{
    Task WriteSubmission  (PrivateSubmit info);
    Task DeleteSubmission (string id);
    
    Task MarkAsPending(string subid);
    Task MarkAsReviewed(string subid, string link);
}