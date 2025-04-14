using ReleaseManager.Core.Models;

namespace ReleaseManager.Core.Interfaces
{
    public interface IPipelineService
    {
        Task<IEnumerable<Pipeline>> GetPipelinesAsync(string projectId);
        Task<Pipeline> GetPipelineByIdAsync(string projectId, string pipelineId);
        Task<PipelineRun> RunPipelineAsync(string projectId, string pipelineId, PipelineRunOptions options);
        Task<IEnumerable<PipelineRun>> GetPipelineRunsAsync(string projectId, string pipelineId, int limit = 10);
    }
}
