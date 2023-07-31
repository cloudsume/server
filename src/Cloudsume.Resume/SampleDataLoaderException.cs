namespace Cloudsume.Resume;

using System;

public class SampleDataLoaderException : Exception
{
    public SampleDataLoaderException()
    {
    }

    public SampleDataLoaderException(string? message)
        : base(message)
    {
    }

    public SampleDataLoaderException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
