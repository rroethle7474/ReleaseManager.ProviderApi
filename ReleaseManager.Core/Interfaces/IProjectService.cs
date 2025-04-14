using ReleaseManager.Core.Models;

namespace ReleaseManager.Core.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsAsync();
        Task<Project> GetProjectByIdAsync(string projectId);
    }
}
