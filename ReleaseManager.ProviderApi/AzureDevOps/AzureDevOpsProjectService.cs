using ReleaseManager.ProviderApi.AzureDevOps;
using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;

namespace ReleaseManager.ProviderApi.AzureDevOps
{
    public class AzureDevOpsProjectService : IProjectService
    {
        private readonly AzureDevOpsClientWrapper _client;

        public AzureDevOpsProjectService(IHttpClientFactory httpClientFactory, CloudProviderCredentials credentials)
        {
            _client = new AzureDevOpsClientWrapper(httpClientFactory, credentials);
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            var response = await _client.GetAsync<AzureDevOpsModels.ProjectListResponse>("_apis/projects");

            return response.Value.Select(p => new Project
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Url = p.Url,
                LastUpdateTime = p.LastUpdateTime
            });
        }

        public async Task<Project> GetProjectByIdAsync(string projectId)
        {
            var project = await _client.GetAsync<AzureDevOpsModels.Project>($"_apis/projects/{projectId}");

            return new Project
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Url = project.Url,
                LastUpdateTime = project.LastUpdateTime
            };
        }
    }
}
