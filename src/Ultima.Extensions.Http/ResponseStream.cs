namespace Ultima.Extensions.Http
{
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// Encapsuates a <see cref="HttpResponseMessage"/> in a <see cref="Stream"/>.
    /// </summary>
    /// <remarks>
    /// The use cases for this class is when you want to encapsulate a content of <see cref="HttpResponseMessage"/> into <see cref="Stream"/> and want
    /// to dispose <see cref="HttpResponseMessage"/> automatically when the <see cref="Stream"/> get disposed.
    /// </remarks>
    public sealed class ResponseStream : Stream
    {
        private readonly HttpResponseMessage response;
        private readonly Stream body;
        private bool disposed;

        public ResponseStream(HttpResponseMessage response, Stream body)
        {
            this.response = response;
            this.body = body;
        }

        public override bool CanRead => this.body.CanRead;

        public override bool CanSeek => this.body.CanSeek;

        public override bool CanWrite => this.body.CanWrite;

        public override long Length => this.body.Length;

        public override long Position
        {
            get => this.body.Position;
            set => this.body.Position = value;
        }

        public override void Flush()
        {
            this.body.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.body.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.body.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.body.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.body.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.body.Dispose();
                    this.response.Dispose();
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
