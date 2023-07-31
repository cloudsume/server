namespace Candidate.Server.Resume.Builder.Renderers
{
    using System.Globalization;
    using Candidate.Server.Resume.Builder.Values;
    using Cloudsume.Builder;

    internal sealed class MonthRenderer : AttributeRenderer
    {
        public MonthRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (Month)obj;
            var display = culture.DateTimeFormat.GetAbbreviatedMonthName(value.Value.Month);

            return this.ToTex(display);
        }
    }
}
