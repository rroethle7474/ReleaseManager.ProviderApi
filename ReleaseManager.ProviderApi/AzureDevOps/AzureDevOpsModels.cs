namespace ReleaseManager.ProviderApi.AzureDevOps
{
    public class AzureDevOpsModels
    {
        public class ProjectListResponse
        {
            public int Count { get; set; }
            public List<Project> Value { get; set; }
        }

        public class Project
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public DateTime LastUpdateTime { get; set; }
        }

        public class PipelineListResponse
        {
            public int Count { get; set; }
            public List<Pipeline> Value { get; set; }
        }

        public class Pipeline
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class PipelineRunListResponse
        {
            public int Count { get; set; }
            public List<PipelineRun> Value { get; set; }
        }

        public class PipelineRun
        {
            public int Id { get; set; }
            public string State { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? FinishedDate { get; set; }
            public string Url { get; set; }
            public Identity CreatedBy { get; set; }
        }

        public class ReleaseListResponse
        {
            public int Count { get; set; }
            public List<Release> Value { get; set; }
        }

        public class Release
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Description { get; set; }
            public Identity CreatedBy { get; set; }
            public ProjectReference ProjectReference { get; set; }
            public List<ReleaseEnvironment> Environments { get; set; }
            public LinkReferences Links { get; set; }
        }

        public class LinkReferences
        {
            public Link Web { get; set; }
        }

        public class Link
        {
            public string Href { get; set; }
        }

        public class ProjectReference
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class ReleaseEnvironment
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public DateTime? StartedOn { get; set; }
            public DateTime? CompletedOn { get; set; }
        }

        public class Identity
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string UniqueName { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
        }

        public class RunPipelineRequest
        {
            public Dictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
            public Resources Resources { get; set; } = new Resources();
        }

        public class Variable
        {
            public string Value { get; set; }
        }

        public class Resources
        {
            public Dictionary<string, Repository> Repositories { get; set; } = new Dictionary<string, Repository>();
        }

        public class Repository
        {
            public string RefName { get; set; }
            public string Version { get; set; }
        }

        public class CreateReleaseRequest
        {
            public ReleaseDefinitionReference Definition { get; set; }
            public string Description { get; set; }
            public bool IsDraft { get; set; }
            public Dictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
            public List<int> EnvironmentIds { get; set; } = new List<int>();
        }

        public class ReleaseDefinitionReference
        {
            public string Id { get; set; }
        }
    }
}