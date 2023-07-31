namespace Ultima.Extensions.Graphics
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class ImageData : IAsyncDisposable, IDisposable
    {
        private readonly bool leaveOpen;
        private bool disposed;

        public ImageData(ImageFormat format, Stream data, int size, bool leaveOpen)
        {
            if (size <= 0)
            {
                throw new ArgumentException("The value is negative or zero.", nameof(size));
            }

            this.Format = format;
            this.Data = data;
            this.Size = size;
            this.leaveOpen = leaveOpen;
        }

        public ImageFormat Format { get; }

        public Stream Data { get; }

        public int Size { get; }

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
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!this.leaveOpen)
                    {
                        this.Data.Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (!this.disposed)
            {
                if (!this.leaveOpen)
                {
                    await this.Data.DisposeAsync();
                }
            }
        }
    }
}
