namespace Cloudsume.Resume
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for content of the asset in a template workspace.
    /// </summary>
    public interface IWorkspaceAssetRepository
    {
        Task WriteAsync(Guid registrationId, AssetName name, Stream content, long size, CancellationToken cancellationToken = default);

        Task<Stream?> GetAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default);
    }
}
