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
    using NetUlid;
    using Ultima.Extensions.Graphics;

    public sealed class FileSystemPreviewRepository : ITemplatePreviewRepository
    {
        private static readonly TypeConverter FormatConverter = TypeDescriptor.GetConverter(typeof(ImageFormat));

        private readonly FileSystemPreviewRepositoryOptions options;

        public FileSystemPreviewRepository(IOptions<FileSystemPreviewRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public async Task<int> CreateAsync(Ulid template, IAsyncEnumerable<Thumbnail> previews, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(template);
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

        public async Task<Thumbnail?> GetAsync(Ulid template, int page, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(template);

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

        public Task<int> GetPageCountAsync(Ulid template, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(template);
            var pages = Directory.EnumerateFiles(directory).Count();

            return Task.FromResult(pages);
        }

        private string GetPath(Ulid template)
        {
            return Path.Join(this.options.Path, template.ToString());
        }

        private string GetFileName(Thumbnail preview)
        {
            return this.GetFileName(preview.Page, preview.Content.Format);
        }

        private string GetFileName(int page, ImageFormat format)
        {
            var name = page.ToString(CultureInfo.InvariantCulture);
            var extension = FormatConverter.ConvertToInvariantString(format);

            return $"{name}.{extension}";
        }
    }
}
