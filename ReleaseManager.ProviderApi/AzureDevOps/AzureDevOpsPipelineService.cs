using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;


namespace ReleaseManager.ProviderApi.AzureDevOps
{
    public class AzureDevOpsPipelineService : IPipelineService
    {
        private readonly AzureDevOpsClientWrapper _client;

        public AzureDevOpsPipelineService(IHttpClientFactory httpClientFactory, CloudProviderCredentials credentials)
        {
            _client = new AzureDevOpsClientWrapper(httpClientFactory, credentials);
        }

        public async Task<IEnumerable<Pipeline>> GetPipelinesAsync(string projectId)
        {
            var response = await _client.GetAsync<AzureDevOpsModels.PipelineListResponse>($"{projectId}/_apis/pipelines");

            return response.Value.Select(p => new Pipeline
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Url = p.Url,
                ProjectId = projectId,
                CreatedDate = p.CreatedDate,
                ProviderType = "AzureDevOps"
            });
        }

        public async Task<Pipeline> GetPipelineByIdAsync(string projectId, string pipelineId)
        {
            var pipeline = await _client.GetAsync<AzureDevOpsModels.Pipeline>($"{projectId}/_apis/pipelines/{pipelineId}");

            return new Pipeline
            {
                Id = pipeline.Id.ToString(),
                Name = pipeline.Name,
                Url = pipeline.Url,
                ProjectId = projectId,
                CreatedDate = pipeline.CreatedDate,
                ProviderType = "AzureDevOps"
            };
        }

        public async Task<PipelineRun> RunPipelineAsync(string projectId, string pipelineId, PipelineRunOptions options)
        {
            var requestBody = new AzureDevOpsModels.RunPipelineRequest
            {
                Variables = options.Variables.Select(kv => new KeyValuePair<string, AzureDevOpsModels.Variable>(
                    kv.Key, new AzureDevOpsModels.Variable { Value = kv.Value })).ToDictionary(kv => kv.Key, kv => kv.Value),
                Resources = new AzureDevOpsModels.Resources
                {
                    Repositories = new Dictionary<string, AzureDevOpsModels.Repository>
                    {
                        ["self"] = new AzureDevOpsModels.Repository
                        {
                            RefName = options.SourceBranch,
                            Version = options.CommitId
                        }
                    }
                }
            };

            var run = await _client.PostAsync<AzureDevOpsModels.PipelineRun>(
                $"{projectId}/_apis/pipelines/{pipelineId}/runs",
                requestBody);

            return new PipelineRun
            {
                Id = run.Id.ToString(),
                PipelineId = pipelineId,
                Status = MapStatus(run.State),
                StartTime = run.CreatedDate,
                EndTime = run.FinishedDate,
                TriggeredBy = run.CreatedBy?.DisplayName ?? "Unknown",
                Url = run.Url
            };
        }

        public async Task<IEnumerable<PipelineRun>> GetPipelineRunsAsync(string projectId, string pipelineId, int limit = 10)
        {
            var response = await _client.GetAsync<AzureDevOpsModels.PipelineRunListResponse>(
                $"{projectId}/_apis/pipelines/{pipelineId}/runs?$top={limit}");

            return response.Value.Select(r => new PipelineRun
            {
                Id = r.Id.ToString(),
                PipelineId = pipelineId,
                Status = MapStatus(r.State),
                StartTime = r.CreatedDate,
                EndTime = r.FinishedDate,
                TriggeredBy = r.CreatedBy?.DisplayName ?? "Unknown",
                Url = r.Url
            });
        }

        private PipelineStatus MapStatus(string state)
        {
            return state?.ToLowerInvariant() switch
            {
                "inprogress" => PipelineStatus.InProgress,
                "completed" => PipelineStatus.Completed,
                "canceling" => PipelineStatus.InProgress,
                "canceled" => PipelineStatus.Cancelled,
                "failed" => PipelineStatus.Failed,
                "queued" => PipelineStatus.Queued,
                _ => PipelineStatus.Unknown
            };
        }
    }
}
