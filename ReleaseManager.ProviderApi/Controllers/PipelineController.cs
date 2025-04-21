using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReleaseManager.Core.Exceptions;
using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;

namespace ReleaseManager.ProviderApi.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PipelinesController : ControllerBase
    {
        private readonly IProviderFactory _providerFactory;
        private readonly ILogger<PipelinesController> _logger;

        public PipelinesController(
            IProviderFactory providerFactory,
            ILogger<PipelinesController> logger)
        {
            _providerFactory = providerFactory;
            _logger = logger;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetPipelines(
            string projectId,
            [FromQuery] int providerId = 1, // Default to Azure DevOps
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = providerId,
                    AuthMethodId = 1 // PAT
                };

                var pipelineService = _providerFactory.CreatePipelineService(providerId, credentials);
                var pipelines = await pipelineService.GetPipelinesAsync(projectId);

                return Ok(pipelines);
            }
            catch (ProviderNotFoundException ex)
            {
                _logger.LogError(ex, "Provider not found");
                return BadRequest(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "Authentication error");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipelines");
                return StatusCode(500, new { message = "An error occurred while retrieving pipelines" });
            }
        }

        [HttpGet("{projectId}/{pipelineId}")]
        public async Task<IActionResult> GetPipelineById(
            string projectId,
            string pipelineId,
            [FromQuery] int providerId = 1, // Default to Azure DevOps
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = providerId,
                    AuthMethodId = 1
                };

                var pipelineService = _providerFactory.CreatePipelineService(providerId, credentials);
                var pipeline = await pipelineService.GetPipelineByIdAsync(projectId, pipelineId);

                return Ok(pipeline);
            }
            catch (ProviderNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline {PipelineId}", pipelineId);
                return StatusCode(500, new { message = "An error occurred while retrieving the pipeline" });
            }
        }

        [HttpPost("{projectId}/{pipelineId}/runs")]
        public async Task<IActionResult> RunPipeline(
            string projectId,
            string pipelineId,
            [FromBody] PipelineRunOptions options,
            [FromQuery] int providerId = 1, // Default to Azure DevOps
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = providerId,
                    AuthMethodId = 1
                };

                var pipelineService = _providerFactory.CreatePipelineService(providerId, credentials);
                var run = await pipelineService.RunPipelineAsync(projectId, pipelineId, options);

                return Ok(run);
            }
            catch (ProviderNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running pipeline {PipelineId}", pipelineId);
                return StatusCode(500, new { message = "An error occurred while running the pipeline" });
            }
        }

        [HttpGet("{projectId}/{pipelineId}/runs")]
        public async Task<IActionResult> GetPipelineRuns(
            string projectId,
            string pipelineId,
            [FromQuery] int limit = 10,
            [FromQuery] int providerId = 1, // Default to Azure DevOps
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = providerId,
                    AuthMethodId = 1
                };

                var pipelineService = _providerFactory.CreatePipelineService(providerId, credentials);
                var runs = await pipelineService.GetPipelineRunsAsync(projectId, pipelineId, limit);

                return Ok(runs);
            }
            catch (ProviderNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline runs for {PipelineId}", pipelineId);
                return StatusCode(500, new { message = "An error occurred while retrieving pipeline runs" });
            }
        }
    }
}