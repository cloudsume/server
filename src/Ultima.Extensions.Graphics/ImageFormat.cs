namespace Ultima.Extensions.Graphics
{
    using System.ComponentModel;

    [TypeConverter(typeof(ImageFormatConverter))]
    public enum ImageFormat
    {
        JPEG = 0,
    }
}
