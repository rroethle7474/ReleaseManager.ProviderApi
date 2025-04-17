using ReleaseManager.Core.Exceptions;
using ReleaseManager.Core.Models;
using ReleaseManager.ProviderApi.Clients;
using System.Text.Json;

namespace ReleaseManager.ProviderApi.Services
{

    public class TokenService : ITokenService
    {
        private readonly IIdentityApiClient _identityApiClient;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IIdentityApiClient identityApiClient,
            ILogger<TokenService> logger)
        {
            _identityApiClient = identityApiClient;
            _logger = logger;
        }

        public async Task<string> GetCloudProviderTokenAsync(int providerId)
        {
            try
            {
                var tokenResponse = await _identityApiClient.GetCloudProviderTokenAsync(providerId);
                return tokenResponse?.Token;
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving token");
                throw new AuthenticationException("An unexpected error occurred while retrieving token", ex);
            }
        }
    }
}