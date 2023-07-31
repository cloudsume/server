namespace Ultima.Extensions.Graphics
{
    using System;

    public static class ImageFormatExtensions
    {
        /// <summary>
        /// Get file extension for the current format.
        /// </summary>
        /// <param name="format">
        /// A format to get extension.
        /// </param>
        /// <returns>
        /// Only file extension, no period at the begining.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="format"/> is not a valid value for <see cref="ImageFormat"/>.
        /// </exception>
        public static string GetFileExtension(this ImageFormat format) => format switch
        {
            ImageFormat.JPEG => "jpeg",
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };

        public static string GetContentType(this ImageFormat format) => format switch
        {
            ImageFormat.JPEG => "image/jpeg",
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
    }
}
