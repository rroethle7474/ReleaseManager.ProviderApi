namespace ReleaseManager.Core.Models
{
    public enum EnvironmentStatus
    {
        NotStarted,
        InProgress,
        Succeeded,
        Canceled,
        Rejected,
        Queued,
        Scheduled,
        PartiallySucceeded
    }

    public class ReleaseEnvironment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EnvironmentStatus Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
