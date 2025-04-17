namespace ReleaseManager.ProviderApi.Services
{
    public interface ITokenService
    {
        Task<string> GetCloudProviderTokenAsync(int providerId);
    }
}
