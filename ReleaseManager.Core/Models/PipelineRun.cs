namespace ReleaseManager.Core.Models
{
    public enum PipelineStatus
    {
        Unknown,
        Queued,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }

    public class PipelineRun
    {
        public string Id { get; set; }
        public string PipelineId { get; set; }
        public PipelineStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TriggeredBy { get; set; }
        public string Url { get; set; }
    }
}
