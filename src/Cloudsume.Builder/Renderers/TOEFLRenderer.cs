namespace Candidate.Server.Resume.Builder.Renderers
{
    using System;
    using System.Globalization;
    using Cloudsume.Builder;
    using Cloudsume.Resume.Data;

    internal sealed class TOEFLRenderer : AttributeRenderer
    {
        public TOEFLRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (TOEFL)obj;
            var display = culture.TwoLetterISOLanguageName switch
            {
                "en" => $"TOEFL {value.Score.ToString(culture)}",
                "th" => $"โทเฟิล {value.Score.ToString(CultureInfo.InvariantCulture)}",
                _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
            };

            return this.ToTex(display);
        }
    }
}
