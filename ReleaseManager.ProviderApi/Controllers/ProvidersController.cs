using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReleaseManager.ProviderApi.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProvidersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProviders()
        {
            // This would typically come from a database or configuration
            var providers = new[]
            {
                new { Id = 1, Name = "AzureDevOps", Description = "Microsoft Azure DevOps Services" },
                // Add more providers as needed
            };

            return Ok(providers);
        }
    }
}