namespace System.IO
{
    using System;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        /// <summary>
        /// Copy one <see cref="Stream"/> to another <see cref="Stream"/> no more than the specified amount of bytes.
        /// </summary>
        /// <param name="source">
        /// Source stream.
        /// </param>
        /// <param name="dest">
        /// Destination stream.
        /// </param>
        /// <param name="amount">
        /// Amount of bytes to copy.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The number of bytes copied. The value may less than <paramref name="amount"/> if the remaining bytes in <paramref name="source"/> less than the
        /// specified value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="amount"/> is less than zero.
        /// </exception>
        public static async ValueTask<long> TransferAsync(this Stream source, Stream dest, long amount, CancellationToken cancellationToken = default)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            // Start transfer.
            using var memory = MemoryPool<byte>.Shared.Rent();
            var buffer = memory.Memory;
            var transferred = 0L;

            while (transferred < amount)
            {
                // Read from source.
                var target = (int)Math.Min(amount - transferred, buffer.Length);
                var result = await source.ReadAsync(buffer[..target], cancellationToken);

                if (result == 0)
                {
                    break;
                }

                // Write to destination.
                await dest.WriteAsync(buffer[..result]);
                transferred += result;
            }

            return transferred;
        }
    }
}
