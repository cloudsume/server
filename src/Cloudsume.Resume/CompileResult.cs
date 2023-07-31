namespace Cloudsume.Resume
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class CompileResult : IAsyncDisposable, IDisposable
    {
        private bool disposed;

        public CompileResult(string source, Stream pdf)
        {
            this.Source = source;
            this.PDF = pdf;
        }

        public string Source { get; }

        public Stream PDF { get; }

        public bool LeaveOpen { get; set; }

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
                    if (!this.LeaveOpen)
                    {
                        this.PDF.Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (!this.disposed)
            {
                if (!this.LeaveOpen)
                {
                    await this.PDF.DisposeAsync();
                }
            }
        }
    }
}
