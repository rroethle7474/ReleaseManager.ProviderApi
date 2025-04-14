namespace ReleaseManager.Core.Models
{
    public class PipelineRunOptions
    {
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
        public string SourceBranch { get; set; }
        public string CommitId { get; set; }
    }
}
