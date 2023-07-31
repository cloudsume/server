namespace Candidate.Server.Resume.Builder.Renderers
{
    using System;
    using System.Globalization;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Builder;

    internal sealed class TOEICRenderer : AttributeRenderer
    {
        public TOEICRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (TOEIC)obj;
            var display = culture.TwoLetterISOLanguageName switch
            {
                "en" => $"TOEIC {value.Score.ToString(culture)}",
                "th" => $"โทอิก {value.Score.ToString(CultureInfo.InvariantCulture)}",
                _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
            };

            return this.ToTex(display);
        }
    }
}
