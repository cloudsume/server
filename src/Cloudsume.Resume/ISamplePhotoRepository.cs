namespace Cloudsume.Resume;

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ISamplePhotoRepository
{
    Task<Stream?> ReadAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default);

    Task WriteAsync(Guid userId, Guid jobId, CultureInfo culture, Stream photo, int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a photo if exists.
    /// </summary>
    /// <param name="userId">
    /// The owner of the photo to delete.
    /// </param>
    /// <param name="jobId">
    /// Job of the photo to delete.
    /// </param>
    /// <param name="culture">
    /// Culture of the photo to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous delete operation.
    /// </returns>
    /// <remarks>
    /// If the photo to be deleted does not exist, no exception is thrown.
    /// </remarks>
    Task DeleteAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default);
}
