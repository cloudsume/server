namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NetUlid;

    public interface ITemplateAssetRepository
    {
        IAsyncEnumerable<AssetFile> ReadAsync(Ulid id, CancellationToken cancellationToken = default);

        Task WriteAsync(Ulid id, AssetFile asset, CancellationToken cancellationToken = default);
    }
}
