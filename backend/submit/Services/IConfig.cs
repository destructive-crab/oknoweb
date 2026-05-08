public interface IConfig
{
    public string IDColumn             { get; }
    public string StatusColumn         { get; }
    
    public string NameColumn           { get; }
    public string LinkColumn           { get; }
    public string DateColumn           { get; }
    public string ContactColumn        { get; }
    public string AdditionalInfoColumn { get; }
    public string VideoLinkColumn      { get; }

    public string DatabasePath       { get; }
    public string UsersDatabasePath  { get; }
}
