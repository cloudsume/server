namespace Cloudsume.Builder.Renderers;

using System.Globalization;

internal sealed class TexStringRenderer : AttributeRenderer
{
    public TexStringRenderer(CultureInfo templateCulture)
        : base(templateCulture)
    {
    }

    public override string Render(object obj, string format, CultureInfo culture)
    {
        return this.ToTex(((TexString)obj).Value);
    }
}
