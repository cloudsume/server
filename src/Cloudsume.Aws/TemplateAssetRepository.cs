namespace Cloudsume.Aws;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Aws;
using Cloudsume.Resume;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetUlid;

public sealed class TemplateAssetRepository : S3Repository, ITemplateAssetRepository
{
    public TemplateAssetRepository(IOptions<TemplateAssetRepositoryOptions> options, ILogger<S3Repository> logger)
        : base(options, logger)
    {
    }

    public async IAsyncEnumerable<AssetFile> ReadAsync(Ulid id, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var prefix = this.GetKeyPrefix(id);

        foreach (var info in await this.ListObjectsAsync(prefix, cancellationToken))
        {
            var name = new AssetName(info.Key[prefix.Length..]);
            var @object = await this.GetObjectAsync(info.Key, cancellationToken);
            AssetFile asset;

            try
            {
                asset = new(name, Convert.ToInt32(@object.ContentLength), new S3ResponseStream(@object));
            }
            catch
            {
                @object.Dispose();
                throw;
            }

            yield return asset;
        }
    }

    public Task WriteAsync(Ulid id, AssetFile asset, CancellationToken cancellationToken = default)
    {
        var key = this.GetKeyPrefix(id) + asset.Name.Value;

        return this.PutObjectAsync(key, asset.Content, asset.Size, cancellationToken);
    }

    private string GetKeyPrefix(Ulid templateId)
    {
        return templateId + KeyDelimiter;
    }
}
