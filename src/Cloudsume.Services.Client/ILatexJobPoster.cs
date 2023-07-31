namespace Cloudsume.Services.Client;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ILatexJobPoster : IAsyncDisposable, IDisposable
{
    Task WriteAssetAsync(string name, int size, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compile all uploaded assets.
    /// </summary>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// Raw PDF content.
    /// </returns>
    /// <exception cref="LatexJobException">
    /// Compilation error occured.
    /// </exception>
    Task<Stream> FinishAsync(CancellationToken cancellationToken = default);
}
