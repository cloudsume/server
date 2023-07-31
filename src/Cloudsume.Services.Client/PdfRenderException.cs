namespace Cloudsume.Services.Client;

using System;

public class PdfRenderException : Exception
{
    public PdfRenderException(string message)
        : base(message)
    {
    }

    public PdfRenderException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
