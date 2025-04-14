using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;

namespace ReleaseManager.ProviderApi.AzureDevOps
{
    public class AzureDevOpsReleaseService : IReleaseService
    {
        private readonly AzureDevOpsClientWrapper _client;

        public AzureDevOpsReleaseService(IHttpClientFactory httpClientFactory, CloudProviderCredentials credentials)
        {
            _client = new AzureDevOpsClientWrapper(httpClientFactory, credentials);
        }

        public async Task<IEnumerable<Release>> GetReleasesAsync(string projectId)
        {
            var response = await _client.GetAsync<AzureDevOpsModels.ReleaseListResponse>(
                $"{projectId}/_apis/release/releases");

            return response.Value.Select(MapToRelease);
        }

        public async Task<Release> GetReleaseByIdAsync(string projectId, string releaseId)
        {
            var release = await _client.GetAsync<AzureDevOpsModels.Release>(
                $"{projectId}/_apis/release/releases/{releaseId}");

            return MapToRelease(release);
        }

        public async Task<Release> CreateReleaseAsync(string projectId, ReleaseOptions options)
        {
            var requestBody = new AzureDevOpsModels.CreateReleaseRequest
            {
                Definition = new AzureDevOpsModels.ReleaseDefinitionReference { Id = options.ArtifactId },
                Description = options.Description,
                IsDraft = false,
                Variables = options.Variables.ToDictionary(
                    kv => kv.Key,
                    kv => new AzureDevOpsModels.Variable { Value = kv.Value }),
                EnvironmentIds = options.EnvironmentIds.Select(int.Parse).ToList()
            };

            var release = await _client.PostAsync<AzureDevOpsModels.Release>(
                $"{projectId}/_apis/release/releases",
                requestBody);

            return MapToRelease(release);
        }

        public async Task<bool> DeleteReleaseAsync(string projectId, string releaseId)
        {
            try
            {
                await _client.PostAsync<object>(
                    $"{projectId}/_apis/release/releases/{releaseId}",
                    new { status = "abandoned" });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private Release MapToRelease(AzureDevOpsModels.Release release)
        {
            return new Release
            {
                Id = release.Id.ToString(),
                Name = release.Name,
                ProjectId = release.ProjectReference?.Id,
                Status = MapReleaseStatus(release.Status),
                CreatedDate = release.CreatedOn,
                CreatedBy = release.CreatedBy?.DisplayName,
                Description = release.Description,
                Environments = release.Environments?.Select(env => new ReleaseEnvironment
                {
                    Id = env.Id.ToString(),
                    Name = env.Name,
                    Status = MapEnvironmentStatus(env.Status),
                    StartTime = env.StartedOn,
                    EndTime = env.CompletedOn
                }).ToList() ?? new List<ReleaseEnvironment>(),
                Url = release.Links?.Web?.Href
            };
        }

        private ReleaseStatus MapReleaseStatus(string status)
        {
            return status?.ToLowerInvariant() switch
            {
                "draft" => ReleaseStatus.Draft,
                "abandoned" => ReleaseStatus.Abandoned,
                _ => ReleaseStatus.Active
            };
        }

        private EnvironmentStatus MapEnvironmentStatus(string status)
        {
            return status?.ToLowerInvariant() switch
            {
                "inprogress" => EnvironmentStatus.InProgress,
                "queued" => EnvironmentStatus.Queued,
                "rejected" => EnvironmentStatus.Rejected,
                "scheduled" => EnvironmentStatus.Scheduled,
                "succeeded" => EnvironmentStatus.Succeeded,
                "partiallysucceeded" => EnvironmentStatus.PartiallySucceeded,
                "canceled" => EnvironmentStatus.Canceled,
                _ => EnvironmentStatus.NotStarted
            };
        }
    }
}
