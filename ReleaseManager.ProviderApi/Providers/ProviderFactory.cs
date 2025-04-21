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

        public IPipelineService CreatePipelineService(int providerId, CloudProviderCredentials credentials)
        {
            switch (providerId)
            {
                case 1: // AzureDevOps
                    return new AzureDevOpsPipelineService(_httpClientFactory, credentials);
                // Add more providers as needed
                default:
                    throw new ProviderNotFoundException($"Unknown providerId: {providerId}");
            }
        }

        public IReleaseService CreateReleaseService(int providerId, CloudProviderCredentials credentials)
        {
            switch (providerId)
            {
                case 1: // AzureDevOps
                    return new AzureDevOpsReleaseService(_httpClientFactory, credentials);
                // Add more providers as needed
                default:
                    throw new ProviderNotFoundException($"Unknown providerId: {providerId}");
            }
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

        public IProjectService CreateProjectService(int providerId, CloudProviderCredentials credentials)
        {
            switch (providerId)
            {
                case 1: // AzureDevOps
                    return new AzureDevOpsProjectService(_httpClientFactory, credentials);
                // Add more providers as needed
                default:
                    throw new ProviderNotFoundException($"Unknown providerId: {providerId}");
            }
        }
    }
}