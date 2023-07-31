namespace Ultima.Extensions.Globalization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class SubdivisionCodeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value switch
            {
                string v => SubdivisionCode.Parse(v),
                _ => base.ConvertFrom(context, culture, value),
            };
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((SubdivisionCode?)value)?.Value;
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
