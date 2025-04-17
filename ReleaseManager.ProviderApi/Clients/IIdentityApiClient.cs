using ReleaseManager.Core.Models;

namespace ReleaseManager.ProviderApi.Clients
{
    public interface IIdentityApiClient
    {
        Task<TokenResponse> GetCloudProviderTokenAsync(int providerId);
        Task<TokenResponse> RefreshTokenAsync(string userId);
    }
}
