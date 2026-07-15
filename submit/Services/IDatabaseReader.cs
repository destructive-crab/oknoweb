namespace Submit; 

public interface IDatabaseReader
{
    Task<bool>            ValidateUser  (string username, string password);
    Task<bool>            HasSubmission (string id);
    Task<PrivateSubmit>   Read          (string id);
    Task<PrivateSubmit[]> ReadAll       ();
}
