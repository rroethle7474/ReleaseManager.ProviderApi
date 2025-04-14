using ReleaseManager.Core.Exceptions;
using System.Text.Json;

namespace ReleaseManager.ProviderApi.Services
{
    public interface ITokenService
    {
        Task<string> GetCloudProviderTokenAsync(Guid userId, int providerId);
    }

    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TokenService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetCloudProviderTokenAsync(Guid userId, int providerId)
        {
            try
            {
                // Configure the Identity API endpoint from configuration
                string identityApiBaseUrl = _configuration["IdentityApi:BaseUrl"];

                // Make request to your Identity API to get the token
                var response = await _httpClient.GetAsync($"{identityApiBaseUrl}/api/CloudProvider/token/{providerId}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to retrieve token. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    throw new AuthenticationException("Failed to retrieve cloud provider token");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse?.Token;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while retrieving token");
                throw new AuthenticationException("Failed to communicate with identity service", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing token response");
                throw new AuthenticationException("Invalid response from identity service", ex);
            }
            catch (Exception ex) when (ex is not AuthenticationException)
            {
                _logger.LogError(ex, "Unexpected error retrieving token");
                throw new AuthenticationException("An unexpected error occurred while retrieving token", ex);
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}