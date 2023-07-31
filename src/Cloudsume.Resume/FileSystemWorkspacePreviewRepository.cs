namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Ultima.Extensions.Graphics;

    public sealed class FileSystemWorkspacePreviewRepository : IWorkspacePreviewRepository
    {
        private static readonly TypeConverter FormatConverter = TypeDescriptor.GetConverter(typeof(ImageFormat));

        private readonly FileSystemWorkspacePreviewRepositoryOptions options;

        public FileSystemWorkspacePreviewRepository(IOptions<FileSystemWorkspacePreviewRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public async Task<Thumbnail?> GetAsync(Guid registrationId, int page, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(registrationId);

            foreach (var format in Enum.GetValues<ImageFormat>())
            {
                var path = Path.Join(directory, this.GetFileName(page, format));

                if (!File.Exists(path))
                {
                    continue;
                }

                var data = File.OpenRead(path);
                ImageData image;

                try
                {
                    image = new ImageData(format, data, Convert.ToInt32(data.Length), false);
                }
                catch
                {
                    await data.DisposeAsync();
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

            return null;
        }

        public Task<int> GetPageCountAsync(Guid registrationId, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(registrationId);
            int pages;

            try
            {
                pages = Directory.EnumerateFiles(directory).Count();
            }
            catch (DirectoryNotFoundException)
            {
                pages = 0;
            }

            return Task.FromResult(pages);
        }

        public async Task<int> UpdateAsync(Guid registrationId, IAsyncEnumerable<Thumbnail> previews, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(registrationId);

            // Remove old previews.
            try
            {
                Directory.Delete(directory, true);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore.
            }

            // Write new previews.
            var count = 0;

            Directory.CreateDirectory(directory);

            await foreach (var preview in previews)
            {
                try
                {
                    var path = Path.Join(directory, this.GetFileName(preview));
                    await using var file = File.OpenWrite(path);

                    await preview.Content.Data.CopyToAsync(file);
                }
                finally
                {
                    await preview.DisposeAsync();
                }

                count++;
            }

            return count;
        }

        private string GetPath(Guid registrationId)
        {
            return Path.Join(this.options.Path, registrationId.ToString());
        }

        private string GetFileName(Thumbnail thumbnail)
        {
            return this.GetFileName(thumbnail.Page, thumbnail.Content.Format);
        }

        private string GetFileName(int page, ImageFormat format)
        {
            var name = page.ToString(CultureInfo.InvariantCulture);
            var extension = FormatConverter.ConvertToInvariantString(format);

            return $"{name}.{extension}";
        }
    }
}
