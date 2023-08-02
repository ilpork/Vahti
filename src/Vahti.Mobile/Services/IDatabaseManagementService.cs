namespace Vahti.Mobile.Services
{
    /// <summary>
    /// Defines functionality related to managing database access
    /// </summary>
    public interface IDatabaseManagementService
    {        
        string DatabaseUrl { get; set; }
        string DatabaseSecret { get; set; }
        bool HasDatabaseUrlDefined { get; }
        bool HasStaticConfiguration { get; }
        void SetDatabaseConfiguration();
    }
}
