namespace ReleaseManager.Core.Models
{
    public enum ReleaseStatus
    {
        Draft,
        Active,
        Abandoned
    }

    public class Release
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public ReleaseStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public List<ReleaseEnvironment> Environments { get; set; } = new List<ReleaseEnvironment>();
        public string Url { get; set; }
    }
}
