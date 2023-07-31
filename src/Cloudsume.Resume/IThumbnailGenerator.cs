namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public interface IThumbnailGenerator
    {
        IAsyncEnumerable<Thumbnail> GenerateAsync(Stream pdf, CancellationToken cancellationToken = default);
    }
}
