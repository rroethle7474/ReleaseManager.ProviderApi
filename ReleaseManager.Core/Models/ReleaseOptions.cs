namespace ReleaseManager.Core.Models
{
    public class ReleaseOptions
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ArtifactId { get; set; } // Can be a build ID or other source
        public List<string> EnvironmentIds { get; set; } = new List<string>();
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
    }
}
