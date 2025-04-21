using ReleaseManager.Core.Models;

namespace ReleaseManager.Core.Interfaces
{
    public interface IProviderFactory
    {
        IPipelineService CreatePipelineService(int providerId, CloudProviderCredentials credentials);
        IReleaseService CreateReleaseService(int providerId, CloudProviderCredentials credentials);
        IProjectService CreateProjectService(int providerId, CloudProviderCredentials credentials);
    }
}
