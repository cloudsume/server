namespace Cloudsume.DataOperations;

using System;

internal sealed class RequestException : Exception
{
    public RequestException(string key, string message)
        : base(message)
    {
        this.Key = key;
    }

    public RequestException(string key, string message, Exception? innerException)
        : base(message, innerException)
    {
        this.Key = key;
    }

    public string Key { get; }
}
