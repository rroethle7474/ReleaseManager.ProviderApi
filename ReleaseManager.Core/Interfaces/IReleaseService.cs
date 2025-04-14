using ReleaseManager.Core.Models;

namespace ReleaseManager.Core.Interfaces
{
    public interface IReleaseService
    {
        Task<IEnumerable<Release>> GetReleasesAsync(string projectId);
        Task<Release> GetReleaseByIdAsync(string projectId, string releaseId);
        Task<Release> CreateReleaseAsync(string projectId, ReleaseOptions options);
        Task<bool> DeleteReleaseAsync(string projectId, string releaseId);
    }
}
