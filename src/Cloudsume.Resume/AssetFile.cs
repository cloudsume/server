namespace Cloudsume.Resume;

using System;
using System.IO;
using System.Threading.Tasks;

public sealed class AssetFile : IAsyncDisposable, IDisposable
{
    private bool disposed;

    public AssetFile(AssetName name, int size, Stream content)
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        this.Name = name;
        this.Size = size;
        this.Content = content;
    }

    public AssetName Name { get; }

    public int Size { get; }

    public Stream Content { get; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();
        this.Dispose(false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.Content.Dispose();
        }

        this.disposed = true;
    }

    private ValueTask DisposeAsyncCore()
    {
        if (this.disposed)
        {
            return default;
        }

        return this.Content.DisposeAsync();
    }
}
