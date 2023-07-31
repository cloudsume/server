namespace Cloudsume.Resume
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public sealed class FileSystemWorkspaceAssetRepository : IWorkspaceAssetRepository
    {
        private readonly FileSystemWorkspaceAssetRepositoryOptions options;

        public FileSystemWorkspaceAssetRepository(IOptions<FileSystemWorkspaceAssetRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public Task DeleteAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default)
        {
            var path = name.ToFileSystem(this.options.Path, registrationId.ToString());

            File.Delete(path);

            return Task.CompletedTask;
        }

        public Task<Stream?> GetAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default)
        {
            var path = name.ToFileSystem(this.options.Path, registrationId.ToString());
            Stream? content;

            try
            {
                content = File.OpenRead(path);
            }
            catch (FileNotFoundException)
            {
                content = null;
            }

            return Task.FromResult(content);
        }

        public async Task WriteAsync(Guid registrationId, AssetName name, Stream content, long size, CancellationToken cancellationToken = default)
        {
            var path = name.ToFileSystem(this.options.Path, registrationId.ToString());
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var file = File.Create(path);
            await content.CopyToAsync(file);
        }
    }
}
