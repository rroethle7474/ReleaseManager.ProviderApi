using ReleaseManager.Core.Models;

namespace ReleaseManager.Core.Interfaces
{
    public interface IProviderFactory
    {
        IPipelineService CreatePipelineService(string providerName, CloudProviderCredentials credentials);
        IReleaseService CreateReleaseService(string providerName, CloudProviderCredentials credentials);
        IProjectService CreateProjectService(string providerName, CloudProviderCredentials credentials);
    }
}
