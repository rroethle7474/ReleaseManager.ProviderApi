using ReleaseManager.Core.Exceptions;
using ReleaseManager.Core.Interfaces;
using ReleaseManager.Core.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ReleaseManager.ProviderApi.AzureDevOps
{
    public class AzureDevOpsClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly CloudProviderCredentials _credentials;
        private const string ApiVersion = "api-version=7.0";

        public AzureDevOpsClientWrapper(IHttpClientFactory httpClientFactory, CloudProviderCredentials credentials)
        {
            _credentials = credentials;
            _httpClient = httpClientFactory.CreateClient();

            // Configure base address for Azure DevOps
            _httpClient.BaseAddress = new Uri($"https://dev.azure.com/{credentials.Organization}/");

            // Configure authentication
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{credentials.AccessToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            // Configure headers
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ReleaseManager-API");
        }

        public async Task<T> GetAsync<T>(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{path}?{ApiVersion}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Failed to authenticate with Azure DevOps. Please check your credentials.", ex);
                }

                throw new ReleaseManagerException($"Error communicating with Azure DevOps API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ReleaseManagerException($"Error processing Azure DevOps API response: {ex.Message}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string path, object requestBody)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{path}?{ApiVersion}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Failed to authenticate with Azure DevOps. Please check your credentials.", ex);
                }

                throw new ReleaseManagerException($"Error communicating with Azure DevOps API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ReleaseManagerException($"Error processing Azure DevOps API response: {ex.Message}", ex);
            }
        }
    }
}
    