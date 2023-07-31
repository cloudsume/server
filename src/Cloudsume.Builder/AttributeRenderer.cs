namespace Cloudsume.Builder;

using System.Globalization;
using Candidate.Server.Resume.Builder;
using NetTemplate;

internal abstract class AttributeRenderer : IAttributeRenderer
{
    private readonly CultureInfo templateCulture;

    protected AttributeRenderer(CultureInfo templateCulture)
    {
        this.templateCulture = templateCulture;
    }

    public abstract string Render(object obj, string format, CultureInfo culture);

    string IAttributeRenderer.ToString(object obj, string formatString, CultureInfo culture)
    {
        return this.Render(obj, formatString, culture);
    }

    protected string ToTex(string s) => TexString.Transform(s, this.templateCulture);

    protected MarkdownRenderer CreateMarkdownRenderer() => new(this.templateCulture);
}
