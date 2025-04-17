using ReleaseManager.Core.Exceptions;
using ReleaseManager.Core.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ReleaseManager.ProviderApi.Clients
{
    public class IdentityApiClient : IIdentityApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IdentityApiClient> _logger;

        public IdentityApiClient(HttpClient httpClient, ILogger<IdentityApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TokenResponse> GetCloudProviderTokenAsync(int providerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/CloudProvider/token/{providerId}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to retrieve token. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                    throw new AuthenticationException("Failed to retrieve cloud provider token");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse;
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

        public async Task<TokenResponse> RefreshTokenAsync(string userId)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    "/api/CloudProvider/refresh-token",
                    new StringContent(
                        JsonSerializer.Serialize(new { UserId = userId }),
                        Encoding.UTF8,
                        "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to refresh token. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new AuthenticationException("Token expired and could not be refreshed");
                    }

                    throw new AuthenticationException("Failed to refresh cloud provider token");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while refreshing token");
                throw new AuthenticationException("Failed to communicate with identity service", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing token response");
                throw new AuthenticationException("Invalid response from identity service", ex);
            }
            catch (Exception ex) when (ex is not AuthenticationException)
            {
                _logger.LogError(ex, "Unexpected error refreshing token");
                throw new AuthenticationException("An unexpected error occurred while refreshing token", ex);
            }
        }
    }
}
