namespace Cloudsume.Resume
{
    using System;
    using System.Buffers;
    using System.Linq;
    using System.Security.Cryptography;

    public readonly struct LinkId : IEquatable<LinkId>
    {
        private readonly byte[]? value;

        public LinkId(byte[] value)
        {
            if (value.Length == 0)
            {
                throw new ArgumentException("The value is not a valid link identifier.", nameof(value));
            }

            this.value = value;
        }

        public byte[] Value => this.value ?? throw new InvalidOperationException("The value is a null link.");

        public static bool operator ==(LinkId left, LinkId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LinkId left, LinkId right)
        {
            return !(left == right);
        }

        public static LinkId Generate()
        {
            using var memory = MemoryPool<byte>.Shared.Rent(16);
            var buffer = memory.Memory[..16];

            RandomNumberGenerator.Fill(buffer.Span);

            return new(buffer.ToArray());
        }

        public bool Equals(LinkId other)
        {
            if (this.value == null)
            {
                return other.value == null;
            }
            else if (other.value == null)
            {
                return false;
            }

            return new ReadOnlySpan<byte>(this.value).SequenceEqual(other.value);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((LinkId)obj);
        }

        public override int GetHashCode()
        {
            uint hash = 0;

            if (this.value != null)
            {
                foreach (var v in this.value)
                {
                    hash = (hash << 8) | ((hash & 0xFF000000) >> 24);
                    hash ^= v;
                }
            }

            return (int)hash;
        }

        /// <summary>
        /// Gets a hex string represent this value.
        /// </summary>
        /// <returns>
        /// A hex string represent this value.
        /// </returns>
        /// <remarks>
        /// This is not a value the client will see. This value is used for internal representation only.
        /// </remarks>
        public override string ToString() => this.value is { } value ? Convert.ToHexString(value) : string.Empty;
    }
}
