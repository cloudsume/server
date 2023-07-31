namespace Cloudsume.Identity
{
    using System;

    public class OidcProviderException : Exception
    {
        public OidcProviderException()
        {
        }

        public OidcProviderException(string? message)
            : base(message)
        {
        }

        public OidcProviderException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
