namespace Candidate.Server.Aws
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Options;
    using NetUlid;
    using Ultima.Extensions.Graphics;

    public sealed class TemplatePreviewRepository : ITemplatePreviewRepository
    {
        private const string KeyDelimiter = "/";

        private static readonly TypeConverter FormatConverter = TypeDescriptor.GetConverter(typeof(ImageFormat));

        private readonly TemplatePreviewRepositoryOptions options;
        private readonly AmazonS3Client client;

        public TemplatePreviewRepository(IOptions<TemplatePreviewRepositoryOptions> options)
        {
            this.options = options.Value;
            this.client = new();
        }

        public async Task<int> CreateAsync(Ulid templateId, IAsyncEnumerable<Thumbnail> previews, CancellationToken cancellationToken = default)
        {
            var pages = 0;

            await foreach (var preview in previews)
            {
                try
                {
                    var request = new PutObjectRequest()
                    {
                        AutoCloseStream = false,
                        AutoResetStreamPosition = false,
                        BucketName = this.options.Bucket,
                        ContentType = preview.Content.Format.GetContentType(),
                        InputStream = preview.Content.Data,
                        Key = this.GetKey(templateId, preview),
                    };

                    request.Headers.ContentLength = preview.Content.Size;

                    await this.client.PutObjectAsync(request);
                }
                finally
                {
                    await preview.DisposeAsync();
                }

                pages++;
            }

            return pages;
        }

        public async Task<Thumbnail?> GetAsync(Ulid templateId, int page, CancellationToken cancellationToken = default)
        {
            // Find a target key.
            var keys = await this.GetKeysAsync(templateId, cancellationToken);
            var target = page.ToString(CultureInfo.InvariantCulture);
            var selected = keys
                .Select(k => new { Key = k, Parsed = ParseKey(k) })
                .SingleOrDefault(d => d.Parsed.Name == target);

            if (selected == null)
            {
                return null;
            }

            // Load target object.
            var format = this.GetFormatFromExtension(selected.Parsed.Extension);
            var response = await this.client.GetObjectAsync(this.options.Bucket, selected.Key, cancellationToken);
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

            (string Name, string Extension) ParseKey(string key)
            {
                var name = key.LastIndexOf(KeyDelimiter, StringComparison.Ordinal) + 1;

                if (name == 0)
                {
                    throw new ArgumentException($"{key} is not a valid key for template preview.", nameof(key));
                }

                var extension = key.LastIndexOf('.');

                if (extension <= name || extension == (key.Length - 1))
                {
                    // Don't allow '/.abc' or '/abc.'
                    throw new ArgumentException($"{key} is not a valid key for template preview.", nameof(key));
                }

                return (key.Substring(name, extension - name), key.Substring(extension + 1));
            }
        }

        public async Task<int> GetPageCountAsync(Ulid templateId, CancellationToken cancellationToken = default)
        {
            var keys = await this.GetKeysAsync(templateId, cancellationToken);

            return keys.Count();
        }

        private async Task<IEnumerable<string>> GetKeysAsync(Ulid template, CancellationToken cancellationToken = default)
        {
            var prefix = this.GetKeyPrefix(template);
            var response = await this.client.ListObjectsV2Async(
                new()
                {
                    BucketName = this.options.Bucket,
                    Delimiter = KeyDelimiter,
                    Prefix = prefix,
                },
                cancellationToken);

            return response.S3Objects.Select(o => o.Key).Where(k => k != prefix).ToArray();
        }

        private ImageFormat GetFormatFromExtension(string extension)
        {
            var format = FormatConverter.ConvertFromInvariantString(extension) ?? throw new Exception($"Unexpected result from {FormatConverter.GetType()}.");

            return (ImageFormat)format;
        }

        private string GetKey(Ulid template, Thumbnail preview)
        {
            var prefix = this.GetKeyPrefix(template);
            var name = preview.Page.ToString(CultureInfo.InvariantCulture);
            var extension = FormatConverter.ConvertToInvariantString(preview.Content.Format);

            return $"{prefix}{name}.{extension}";
        }

        private string GetKeyPrefix(Ulid template)
        {
            return template + KeyDelimiter;
        }
    }
}
