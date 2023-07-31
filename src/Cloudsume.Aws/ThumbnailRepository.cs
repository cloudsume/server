namespace Cloudsume.Aws
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Aws;
    using Cloudsume.Resume;
    using Cornot;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Ultima.Extensions.Graphics;

    public abstract class ThumbnailRepository<TId> : S3Repository
    {
        private readonly S3RepositoryOptions options;

        protected ThumbnailRepository(IOptions<S3RepositoryOptions> options, ILogger<S3Repository> logger)
            : base(options, logger)
        {
            this.options = options.Value;
        }

        public async Task<Thumbnail?> GetAsync(TId id, int page, CancellationToken cancellationToken = default)
        {
            // Find a target key.
            var thumbnails = await this.ListObjectsAsync(this.GetKeyPrefix(id), cancellationToken);
            var name = page.ToString(CultureInfo.InvariantCulture);
            string? key = default;
            ImageFormat format = default;

            foreach (var thumbnail in thumbnails)
            {
                var (_, f, e) = ParseKey(thumbnail.Key);

                if (f == name)
                {
                    if (e == null)
                    {
                        throw new DataCorruptionException(thumbnail, "No file name extension.");
                    }

                    key = thumbnail.Key;
                    format = ImageName.GetFormatByExtension(e);
                    break;
                }
            }

            if (key == null)
            {
                return null;
            }

            // Load target object.
            var response = await this.Client.GetObjectAsync(this.options.Bucket, key, cancellationToken);
            S3ResponseStream body;
            ImageData image;

            try
            {
                body = new(response);
            }
            catch
            {
                response.Dispose();
                throw;
            }

            try
            {
                image = new(format, body, Convert.ToInt32(response.ContentLength), false);
            }
            catch
            {
                await body.DisposeAsync();
                throw;
            }

            try
            {
                return new Thumbnail(page, image);
            }
            catch
            {
                await image.DisposeAsync();
                throw;
            }
        }

        public async Task<int> GetPageCountAsync(TId id, CancellationToken cancellationToken = default)
        {
            var thumbnails = await this.ListObjectsAsync(this.GetKeyPrefix(id), cancellationToken);

            return thumbnails.Count();
        }

        public async Task<int> UpdateAsync(TId id, IAsyncEnumerable<Thumbnail> thumbnails, CancellationToken cancellationToken = default)
        {
            // Remove current thumbnails before create a new one.
            await this.DeleteAsync(id, cancellationToken);

            // Create new thumbnails.
            var pages = 0;

            await foreach (var thumbnail in thumbnails)
            {
                try
                {
                    var key = this.GetKeyPrefix(id) + thumbnail.GetFileName();
                    var content = thumbnail.Content.Data;
                    var size = thumbnail.Content.Size;
                    var type = thumbnail.Content.Format.GetContentType();

                    await this.PutObjectAsync(key, content, size, type);
                }
                finally
                {
                    await thumbnail.DisposeAsync();
                }

                pages++;
            }

            return pages;
        }

        protected async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            var objects = await this.ListObjectsAsync(this.GetKeyPrefix(id), cancellationToken);

            await this.DeleteObjectsAsync(objects.Select(o => o.Key), cancellationToken);
        }

        protected string GetKeyPrefix(TId id) => this.GetDirectory(id) + KeyDelimiter;

        protected abstract string GetDirectory(TId id);
    }
}
