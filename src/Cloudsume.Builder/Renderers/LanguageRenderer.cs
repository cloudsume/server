namespace Cloudsume.Builder.Renderers;

using System.Globalization;
using Cloudsume.Builder.Values;

internal sealed class LanguageRenderer : AttributeRenderer
{
    public LanguageRenderer(CultureInfo templateCulture)
        : base(templateCulture)
    {
    }

    public override string Render(object obj, string format, CultureInfo culture)
    {
        var previous = CultureInfo.CurrentUICulture;
        string display;

        try
        {
            CultureInfo.CurrentUICulture = culture;
            display = ((Language)obj).Value.DisplayName;
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }

        return this.ToTex(display);
    }
}
