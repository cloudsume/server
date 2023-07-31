namespace Ultima.Extensions.Graphics
{
    using System;

    public readonly struct ImageName
    {
        public static ImageFormat GetFormatByExtension(string extension)
        {
            if (Equals("jpeg"))
            {
                return ImageFormat.JPEG;
            }
            else
            {
                throw new ArgumentException("Unknow extension.", nameof(extension));
            }

            bool Equals(string expect) => extension.Equals(expect, StringComparison.OrdinalIgnoreCase);
        }
    }
}
