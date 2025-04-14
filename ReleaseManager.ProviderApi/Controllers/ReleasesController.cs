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
    public class ReleasesController : ControllerBase
    {
        private readonly IProviderFactory _providerFactory;
        private readonly ILogger<ReleasesController> _logger;

        public ReleasesController(
            IProviderFactory providerFactory,
            ILogger<ReleasesController> logger)
        {
            _providerFactory = providerFactory;
            _logger = logger;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetReleases(
            string projectId,
            [FromQuery] string providerName = "AzureDevOps",
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = 1,
                    AuthMethodId = 1
                };

                var releaseService = _providerFactory.CreateReleaseService(providerName, credentials);
                var releases = await releaseService.GetReleasesAsync(projectId);

                return Ok(releases);
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
                _logger.LogError(ex, "Error getting releases for project {ProjectId}", projectId);
                return StatusCode(500, new { message = "An error occurred while retrieving releases" });
            }
        }

        [HttpGet("{projectId}/{releaseId}")]
        public async Task<IActionResult> GetReleaseById(
            string projectId,
            string releaseId,
            [FromQuery] string providerName = "AzureDevOps",
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = 1,
                    AuthMethodId = 1
                };

                var releaseService = _providerFactory.CreateReleaseService(providerName, credentials);
                var release = await releaseService.GetReleaseByIdAsync(projectId, releaseId);

                return Ok(release);
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
                _logger.LogError(ex, "Error getting release {ReleaseId}", releaseId);
                return StatusCode(500, new { message = "An error occurred while retrieving the release" });
            }
        }

        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateRelease(
            string projectId,
            [FromBody] ReleaseOptions options,
            [FromQuery] string providerName = "AzureDevOps",
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = 1,
                    AuthMethodId = 1
                };

                var releaseService = _providerFactory.CreateReleaseService(providerName, credentials);
                var release = await releaseService.CreateReleaseAsync(projectId, options);

                return CreatedAtAction(nameof(GetReleaseById), new { projectId, releaseId = release.Id }, release);
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
                _logger.LogError(ex, "Error creating release for project {ProjectId}", projectId);
                return StatusCode(500, new { message = "An error occurred while creating the release" });
            }
        }

        [HttpDelete("{projectId}/{releaseId}")]
        public async Task<IActionResult> DeleteRelease(
            string projectId,
            string releaseId,
            [FromQuery] string providerName = "AzureDevOps",
            [FromQuery] string organization = null)
        {
            try
            {
                var credentials = new CloudProviderCredentials
                {
                    AccessToken = HttpContext.Request.Headers["Provider-Token"],
                    Organization = organization,
                    ProviderId = 1,
                    AuthMethodId = 1
                };

                var releaseService = _providerFactory.CreateReleaseService(providerName, credentials);
                var result = await releaseService.DeleteReleaseAsync(projectId, releaseId);

                if (result)
                {
                    return NoContent();
                }

                return NotFound();
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
                _logger.LogError(ex, "Error deleting release {ReleaseId}", releaseId);
                return StatusCode(500, new { message = "An error occurred while deleting the release" });
            }
        }
    }
}
