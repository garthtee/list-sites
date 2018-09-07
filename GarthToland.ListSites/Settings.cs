namespace GarthToland.ListSites
{
    public interface ISettings
    {
        string LocalHostPath { get; set; }
        string LocalhostUrl { get; set; }
    }

    public class Settings : ISettings
    {
        public string LocalHostPath { get; set; }
        public string LocalhostUrl { get; set; }
    }
}