namespace Candidate.Server.Resume.Builder.Renderers
{
    using System;
    using System.Globalization;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Builder;
    using static Candidate.Server.Resume.Data.IELTS;

    internal sealed class IELTSRenderer : AttributeRenderer
    {
        public IELTSRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var value = (IELTS)obj;
            var result = culture.TwoLetterISOLanguageName switch
            {
                "en" => value.Type switch
                {
                    TypeId.GeneralTraining => $"IELTS {value.Band.ToString(culture)} (General)",
                    TypeId.Academic => $"IELTS {value.Band.ToString(culture)} (Academic)",
                    _ => throw new ArgumentException($"Unknow module type {value.Type}.", nameof(obj)),
                },
                "th" => value.Type switch
                {
                    TypeId.GeneralTraining => $"ไอเอลส์ {value.Band.ToString(CultureInfo.InvariantCulture)} (ทั่วไป)",
                    TypeId.Academic => $"ไอเอลส์ {value.Band.ToString(CultureInfo.InvariantCulture)} (วิชาการ)",
                    _ => throw new ArgumentException($"Unknow module type {value.Type}.", nameof(obj)),
                },
                _ => throw new ArgumentException($"Unsupported culture {culture}.", nameof(culture)),
            };

            return this.ToTex(result);
        }
    }
}
