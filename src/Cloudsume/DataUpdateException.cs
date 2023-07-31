namespace Cloudsume;

using System;

internal class DataUpdateException : Exception
{
    public DataUpdateException()
    {
    }

    public DataUpdateException(string? message)
        : base(message)
    {
    }

    public DataUpdateException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
