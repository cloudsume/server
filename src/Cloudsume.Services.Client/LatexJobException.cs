namespace Cloudsume.Services.Client;

using System;

/// <summary>
/// Represents an error occurred during LaTeX compilation.
/// </summary>
/// <remarks>
/// <see cref="Exception.Message"/> will be contains the compiler output.
/// </remarks>
public class LatexJobException : Exception
{
    public LatexJobException(string error)
        : base(error)
    {
    }

    public LatexJobException(string error, Exception? innerException)
        : base(error, innerException)
    {
    }
}
