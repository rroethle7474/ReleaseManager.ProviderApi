namespace ReleaseManager.Core.Models
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
