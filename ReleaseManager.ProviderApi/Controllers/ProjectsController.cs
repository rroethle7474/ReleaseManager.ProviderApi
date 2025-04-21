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
    public class ProjectsController : ControllerBase
    {
        private readonly IProviderFactory _providerFactory;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProviderFactory providerFactory,
            ILogger<ProjectsController> logger)
        {
            _providerFactory = providerFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects(
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

                var projectService = _providerFactory.CreateProjectService(providerId, credentials);
                var projects = await projectService.GetProjectsAsync();

                return Ok(projects);
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
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, new { message = "An error occurred while retrieving projects" });
            }
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(
            string projectId,
            [FromQuery] int providerId = 1  ,
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

                var projectService = _providerFactory.CreateProjectService(providerId, credentials);
                var project = await projectService.GetProjectByIdAsync(projectId);

                return Ok(project);
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
                _logger.LogError(ex, "Error getting project {ProjectId}", projectId);
                return StatusCode(500, new { message = "An error occurred while retrieving the project" });
            }
        }
    }
}