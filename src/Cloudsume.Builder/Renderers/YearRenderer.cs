namespace Candidate.Server.Resume.Builder.Renderers
{
    using System.Globalization;
    using Candidate.Server.Resume.Builder.Values;
    using Cloudsume.Builder;

    internal sealed class YearRenderer : AttributeRenderer
    {
        public YearRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (Year)obj;
            var calendar = culture.Calendar;
            var display = calendar.GetYear(value.Value).ToString(CultureInfo.InvariantCulture);

            return this.ToTex(display);
        }
    }
}
