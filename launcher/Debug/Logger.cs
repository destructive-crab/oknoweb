using System.Reflection;
using System.Text;

namespace api.Debug;

public interface ILocalLogger
{
    void Message(object message);
    void Error(object message);
}

public sealed class LocalLogger : ILocalLogger
{
    private string LogFilePath => Path.Combine(Utils.GetConfigDirectory(), "app.log");
    
    private const string MESSAGE_PREFIX = "[INFO}";
    private const string ERROR_PREFIX   = "[ERROR}";
    
    private static bool locked = false;
    
    private async void WriteNewLogEntry(string entry)
    {
        while (locked)
        {
            await Task.Yield();
        }

        locked = true;
        {
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Close();
            }
            
            entry = DateTime.Now + " " + entry + "\n";

            await File.AppendAllTextAsync(LogFilePath, entry, Encoding.UTF8);
        }
        locked = false;
    }
    
    public void Message(object message)
    {
        WriteNewLogEntry("[INFO] " + message.ToString());
    }

    public async void Error(object message)
    {
        WriteNewLogEntry("[ERROR] " + message.ToString());
    }
}