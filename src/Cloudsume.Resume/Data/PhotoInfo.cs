namespace Candidate.Server.Resume.Data
{
    using System;
    using Ultima.Extensions.Graphics;

    public sealed class PhotoInfo
    {
        public PhotoInfo(ImageFormat format, int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            this.Format = format;
            this.Size = size;
        }

        public ImageFormat Format { get; }

        public int Size { get; }
    }
}
