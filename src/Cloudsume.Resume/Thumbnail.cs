namespace Cloudsume.Resume
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Ultima.Extensions.Graphics;

    public sealed class Thumbnail : IAsyncDisposable, IDisposable
    {
        private bool disposed;

        public Thumbnail(int page, ImageData content)
        {
            if (page < 0)
            {
                throw new ArgumentException("The value is negative.", nameof(page));
            }

            this.Page = page;
            this.Content = content;
        }

        public int Page { get; }

        public ImageData Content { get; }

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

        public string GetFileName()
        {
            var name = this.Page.ToString(CultureInfo.InvariantCulture);
            var extension = this.Content.Format.GetFileExtension();

            return $"{name}.{extension}";
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Content.Dispose();
                }

                this.disposed = true;
            }
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (!this.disposed)
            {
                await this.Content.DisposeAsync();
            }
        }
    }
}
