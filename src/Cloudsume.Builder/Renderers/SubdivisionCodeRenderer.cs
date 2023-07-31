namespace Cloudsume.Builder.Renderers
{
    using System.Globalization;
    using Cloudsume.Builder;
    using Ultima.Extensions.Globalization;

    internal sealed class SubdivisionCodeRenderer : AttributeRenderer
    {
        public SubdivisionCodeRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            return this.ToTex(((SubdivisionCode)obj).ToString(culture));
        }
    }
}
