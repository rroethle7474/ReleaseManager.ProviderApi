namespace ReleaseManager.Core.Models
{
    public class CloudProviderCredentials
    {
        public string AccessToken { get; set; }
        public string Organization { get; set; }
        public int ProviderId { get; set; }
        public int AuthMethodId { get; set; }
    }
}
