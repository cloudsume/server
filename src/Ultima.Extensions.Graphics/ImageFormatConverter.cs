namespace Ultima.Extensions.Graphics
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class ImageFormatConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string v)
            {
                return ImageName.GetFormatByExtension(v);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value is null ? null : ((ImageFormat)value).GetFileExtension();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
