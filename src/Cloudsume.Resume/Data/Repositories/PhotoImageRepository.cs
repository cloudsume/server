namespace Candidate.Server.Resume.Data.Repositories
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    internal sealed class PhotoImageRepository : IPhotoImageRepository
    {
        private readonly PhotoImageRepositoryOptions options;

        public PhotoImageRepository(IOptions<PhotoImageRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public Task DeleteAsync(Guid userId, object key, CancellationToken cancellationToken = default)
        {
            var path = this.GetPath(userId, key);

            try
            {
                File.Delete(path);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore.
            }

            return Task.CompletedTask;
        }

        public Task<Stream?> GetAsync(Guid userId, object key, CancellationToken cancellationToken = default)
        {
            Stream? image;

            try
            {
                image = File.OpenRead(this.GetPath(userId, key));
            }
            catch (FileNotFoundException)
            {
                image = null;
            }

            return Task.FromResult(image);
        }

        public async Task UpdateAsync(Guid userId, object key, Stream image, int size, CancellationToken cancellationToken = default)
        {
            var path = this.GetPath(userId, key);

            var directory = Path.GetDirectoryName(path);

            Directory.CreateDirectory(directory!);

            await using var file = File.Create(path);
            await image.CopyToAsync(file, cancellationToken);
        }

        private string GetPath(Guid userId, object key)
        {
            var @base = Path.Join(this.options.Path, userId.ToString());

            return key switch
            {
                Guid resumeId => Path.Join(@base, "locals", resumeId.ToString()),
                CultureInfo culture => culture.Name switch
                {
                    "" => Path.Join(@base, "globals", "invariant"),
                    _ => Path.Join(@base, "globals", culture.Name),
                },
                _ => throw new ArgumentException($"Unknown key type {key.GetType()}.", nameof(key)),
            };
        }
    }
}
