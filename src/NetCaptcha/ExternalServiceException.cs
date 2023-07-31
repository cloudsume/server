namespace NetCaptcha;

using System;

public class ExternalServiceException : Exception
{
    public ExternalServiceException()
    {
    }

    public ExternalServiceException(string? message)
        : base(message)
    {
    }

    public ExternalServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
