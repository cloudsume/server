namespace Cloudsume.Aws
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public sealed class WorkspaceAssetRepository : S3Repository, IWorkspaceAssetRepository
    {
        public WorkspaceAssetRepository(IOptions<WorkspaceAssetRepositoryOptions> options, ILogger<S3Repository> logger)
            : base(options, logger)
        {
        }

        public Task DeleteAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default)
        {
            return this.DeleteObjectAsync(this.GetKeyPrefix(registrationId) + name.Value, cancellationToken);
        }

        public async Task<Stream?> GetAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default)
        {
            var key = this.GetKeyPrefix(registrationId) + name.Value;

            try
            {
                return await this.GetObjectContentAsync(key, cancellationToken);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task WriteAsync(Guid registrationId, AssetName name, Stream content, long size, CancellationToken cancellationToken = default)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            return this.PutObjectAsync(this.GetKeyPrefix(registrationId) + name.Value, content, size, cancellationToken);
        }

        private string GetKeyPrefix(Guid registrationId)
        {
            return registrationId + KeyDelimiter;
        }
    }
}
