using System.Security.Claims;

namespace ReleaseManager.ProviderApi.Services
{

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var nameIdentifierClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim == null || !Guid.TryParse(nameIdentifierClaim.Value, out var userId))
            {
                throw new InvalidOperationException("User ID not found in claims");
            }

            return userId;
        }

        public Guid GetOrganizationId()
        {
            var organizationClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("organizationId");
            if (organizationClaim == null || !Guid.TryParse(organizationClaim.Value, out var organizationId))
            {
                throw new InvalidOperationException("Organization ID not found in claims");
            }

            return organizationId;
        }

        public string GetEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new InvalidOperationException("Email not found in claims");
        }

        public string GetFullName()
        {
            var firstName = _httpContextAccessor.HttpContext?.User?.FindFirst("firstName")?.Value;
            var lastName = _httpContextAccessor.HttpContext?.User?.FindFirst("lastName")?.Value;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                throw new InvalidOperationException("User name not found in claims");
            }

            return $"{firstName} {lastName}".Trim();
        }
    }
}