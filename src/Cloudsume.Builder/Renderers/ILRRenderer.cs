namespace Candidate.Server.Resume.Builder.Renderers
{
    using System;
    using System.Globalization;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Builder;
    using static Candidate.Server.Resume.Data.ILR;

    internal sealed class ILRRenderer : AttributeRenderer
    {
        public ILRRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (ILR)obj;
            var display = value.Scale switch
            {
                ScaleId.NoProficiency => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "No proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                ScaleId.Elementary => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "Elementary proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                ScaleId.LimitedWorking => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "Limited working proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                ScaleId.ProfessionalWorking => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "Professional working proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                ScaleId.FullProfessional => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "Full professional proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                ScaleId.Native => culture.TwoLetterISOLanguageName switch
                {
                    "en" => "Native or bilingual proficiency",
                    _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
                },
                _ => throw new ArgumentException($"Unknow scale {value.Scale}.", nameof(obj)),
            };

            return this.ToTex(display);
        }
    }
}
