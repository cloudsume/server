namespace Candidate.Server.Resume
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Options;
    using Ultima.Extensions.Graphics;

    internal sealed class FileSystemThumbnailRepository : IThumbnailRepository
    {
        private static readonly TypeConverter FormatConverter = TypeDescriptor.GetConverter(typeof(ImageFormat));

        private readonly FileSystemThumbnailRepositoryOptions options;

        public FileSystemThumbnailRepository(IOptions<FileSystemThumbnailRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public Task DeleteByResumeAsync(Resume resume, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(resume.Id);

            try
            {
                Directory.Delete(directory, true);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore.
            }

            return Task.CompletedTask;
        }

        public async Task<Thumbnail?> FindByResumeAsync(
            Resume resume,
            int page,
            CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(resume.Id);

            foreach (var format in Enum.GetValues<ImageFormat>())
            {
                var path = Path.Combine(directory, this.GetFileName(page, format));

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

        public async Task<int> UpdateAsync(
            Resume resume,
            IAsyncEnumerable<Thumbnail> thumbnails,
            CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(resume.Id);

            // Remove old thumbnails.
            if (Directory.Exists(directory))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (DirectoryNotFoundException)
                {
                    // Ignore.
                }
            }

            // Write new thumbnails.
            var count = 0;

            Directory.CreateDirectory(directory);

            await foreach (var thumbnail in thumbnails)
            {
                try
                {
                    var path = Path.Combine(directory, this.GetFileName(thumbnail));
                    await using var file = File.OpenWrite(path);

                    await thumbnail.Content.Data.CopyToAsync(file);
                }
                finally
                {
                    await thumbnail.DisposeAsync();
                }

                count++;
            }

            return count;
        }

        public Task<int> GetPageCountAsync(Resume resume, CancellationToken cancellationToken = default)
        {
            var directory = this.GetPath(resume.Id);
            var pages = Directory.EnumerateFiles(directory).Count();

            return Task.FromResult(pages);
        }

        private string GetPath(Guid resumeId)
        {
            return Path.Combine(this.options.Path, resumeId.ToString());
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
