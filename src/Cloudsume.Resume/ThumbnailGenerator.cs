namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Cloudsume.Services.Client;

    public sealed class ThumbnailGenerator : IThumbnailGenerator
    {
        private readonly IServicesClient services;

        public ThumbnailGenerator(IServicesClient services)
        {
            this.services = services;
        }

        public async IAsyncEnumerable<Thumbnail> GenerateAsync(Stream pdf, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var page = 0;

            await foreach (var thumbnail in this.services.RenderPdfAsync(pdf, 1024, cancellationToken))
            {
                Thumbnail result;

                try
                {
                    result = new Thumbnail(page++, thumbnail);
                }
                catch
                {
                    await thumbnail.DisposeAsync();
                    throw;
                }

                yield return result;
            }
        }
    }
}
