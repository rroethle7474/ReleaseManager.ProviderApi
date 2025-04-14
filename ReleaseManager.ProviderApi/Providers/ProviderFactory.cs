using ReleaseManager.ProviderApi.AzureDevOps;
using ReleaseManager.Core.Exceptions;
using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;

namespace ReleaseManager.ProviderApi.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProviderFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IPipelineService CreatePipelineService(string providerName, CloudProviderCredentials credentials)
        {
            return providerName.ToLowerInvariant() switch
            {
                "azuredevops" => new AzureDevOpsPipelineService(_httpClientFactory, credentials),
                // Add more providers as needed
                _ => throw new ProviderNotFoundException(providerName)
            };
        }

        public IReleaseService CreateReleaseService(string providerName, CloudProviderCredentials credentials)
        {
            return providerName.ToLowerInvariant() switch
            {
                "azuredevops" => new AzureDevOpsReleaseService(_httpClientFactory, credentials),
                // Add more providers as needed
                _ => throw new ProviderNotFoundException(providerName)
            };
        }

        public IProjectService CreateProjectService(string providerName, CloudProviderCredentials credentials)
        {
            return providerName.ToLowerInvariant() switch
            {
                "azuredevops" => new AzureDevOpsProjectService(_httpClientFactory, credentials),
                // Add more providers as needed
                _ => throw new ProviderNotFoundException(providerName)
            };
        }
    }
}