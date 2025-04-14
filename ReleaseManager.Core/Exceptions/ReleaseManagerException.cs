namespace ReleaseManager.Core.Exceptions
{
    public class ReleaseManagerException : Exception
    {
        public ReleaseManagerException(string message) : base(message) { }
        public ReleaseManagerException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ProviderNotFoundException : ReleaseManagerException
    {
        public ProviderNotFoundException(string providerName)
            : base($"Provider '{providerName}' is not supported or could not be found.") { }
    }

    public class AuthenticationException : ReleaseManagerException
    {
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
