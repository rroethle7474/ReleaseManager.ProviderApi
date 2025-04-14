namespace ReleaseManager.Core.Models
{
    public class Pipeline
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProviderType { get; set; } // e.g., "AzureDevOps"
    }
}
