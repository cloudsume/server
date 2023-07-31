namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using NetUlid;

    public sealed class FileSystemTemplateAssetRepository : ITemplateAssetRepository
    {
        private readonly FileSystemTemplateAssetRepositoryOptions options;

        public FileSystemTemplateAssetRepository(IOptions<FileSystemTemplateAssetRepositoryOptions> options)
        {
            this.options = options.Value;
        }

        public async IAsyncEnumerable<AssetFile> ReadAsync(Ulid id, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Start listing directory.
            var directory = new DirectoryInfo(Path.Join(this.options.Path, id.ToString()));
            IEnumerable<FileInfo> files;

            try
            {
                files = directory.EnumerateFiles("*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }

            // Load assets.
            foreach (var file in files)
            {
                var name = AssetName.FromFileSystem(file.FullName[(directory.FullName.Length + 1)..]);
                var content = file.OpenRead();
                AssetFile asset;

                try
                {
                    asset = new(name, Convert.ToInt32(file.Length), content);
                }
                catch
                {
                    await content.DisposeAsync();
                    throw;
                }

                yield return asset;
            }
        }

        public async Task WriteAsync(Ulid id, AssetFile asset, CancellationToken cancellationToken = default)
        {
            // Get file path and create parent directories.
            var path = asset.Name.ToFileSystem(this.options.Path, id.ToString());
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write file.
            await using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            await asset.Content.CopyToAsync(file);
        }
    }
}
