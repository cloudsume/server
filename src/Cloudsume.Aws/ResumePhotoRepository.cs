namespace Candidate.Server.Aws
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Candidate.Server.Resume.Data.Repositories;
    using Microsoft.Extensions.Options;

    internal sealed class ResumePhotoRepository : IPhotoImageRepository
    {
        private readonly ResumePhotoRepositoryOptions options;
        private readonly AmazonS3Client client;

        public ResumePhotoRepository(IOptions<ResumePhotoRepositoryOptions> options)
        {
            this.options = options.Value;
            this.client = new AmazonS3Client();
        }

        public Task DeleteAsync(Guid userId, object key, CancellationToken cancellationToken = default)
        {
            var request = new DeleteObjectRequest()
            {
                BucketName = this.options.Bucket,
                Key = this.GetObjectKey(userId, key),
            };

            return this.client.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<Stream?> GetAsync(Guid userId, object key, CancellationToken cancellationToken = default)
        {
            GetObjectResponse response;

            try
            {
                response = await this.client.GetObjectAsync(this.options.Bucket, this.GetObjectKey(userId, key), cancellationToken);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return new S3ResponseStream(response);
        }

        public Task UpdateAsync(Guid userId, object key, Stream image, int size, CancellationToken cancellationToken = default)
        {
            var request = new PutObjectRequest()
            {
                AutoCloseStream = false,
                AutoResetStreamPosition = false,
                BucketName = this.options.Bucket,
                ContentType = "application/octet-stream",
                InputStream = image,
                Key = this.GetObjectKey(userId, key),
            };

            request.Headers.ContentLength = size;

            return this.client.PutObjectAsync(request, cancellationToken);
        }

        private string GetObjectKey(Guid userId, object key)
        {
            var @base = userId.ToString();

            return key switch
            {
                Guid resumeId => $"{@base}/locals/{resumeId}",
                CultureInfo culture => culture.Name switch
                {
                    "" => $"{@base}/globals/invariant",
                    _ => $"{@base}/globals/{culture.Name}",
                },
                _ => throw new ArgumentException($"Unknow key {key.GetType()}.", nameof(key)),
            };
        }
    }
}
