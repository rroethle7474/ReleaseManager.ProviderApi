using System.Net.Http.Json;

namespace ReleaseManager.Core.Utils
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<T> PostJsonAsync<T>(this HttpClient client, string requestUri, object content)
        {
            var response = await client.PostAsJsonAsync(requestUri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
