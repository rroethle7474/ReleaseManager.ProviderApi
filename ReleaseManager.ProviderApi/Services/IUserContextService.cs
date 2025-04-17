namespace ReleaseManager.ProviderApi.Services
{
    public interface IUserContextService
    {
        Guid GetUserId();
        Guid GetOrganizationId();
        string GetEmail();
        string GetFullName();
    }
}
