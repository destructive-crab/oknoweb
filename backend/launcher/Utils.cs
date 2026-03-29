namespace api;

public static class Utils
{
    public static string GetConfigDirectory() => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
}