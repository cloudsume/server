namespace Candidate.Server.Aws
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3.Model;

    internal sealed class S3ResponseStream : Stream
    {
        private readonly StreamResponse response;

        public S3ResponseStream(StreamResponse response)
        {
            this.response = response;
        }

        public override bool CanRead => this.response.ResponseStream.CanRead;

        public override bool CanSeek => this.response.ResponseStream.CanSeek;

        public override bool CanTimeout => this.Body.CanTimeout;

        public override bool CanWrite => this.response.ResponseStream.CanWrite;

        public override long Length => this.response.ResponseStream.Length;

        public override long Position
        {
            get => this.response.ResponseStream.Position;
            set => this.response.ResponseStream.Position = value;
        }

        public override int ReadTimeout
        {
            get => this.Body.ReadTimeout;
            set => this.Body.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => this.Body.WriteTimeout;
            set => this.Body.WriteTimeout = value;
        }

        private Stream Body => this.response.ResponseStream;

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return this.Body.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return this.Body.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            this.Body.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return this.Body.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.Body.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.Body.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            this.response.ResponseStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return this.Body.FlushAsync(cancellationToken);
        }

        public override int Read(Span<byte> buffer)
        {
            return this.Body.Read(buffer);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.response.ResponseStream.Read(buffer, offset, count);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return this.Body.ReadAsync(buffer, cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.Body.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int ReadByte()
        {
            return this.Body.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.response.ResponseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.response.ResponseStream.SetLength(value);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            this.Body.Write(buffer);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.response.ResponseStream.Write(buffer, offset, count);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return this.Body.WriteAsync(buffer, cancellationToken);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.Body.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            this.Body.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.response.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
