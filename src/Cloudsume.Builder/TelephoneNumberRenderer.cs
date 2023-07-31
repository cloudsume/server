namespace Candidate.Server.Resume.Builder
{
    using System.Globalization;
    using Cloudsume.Builder;
    using Ultima.Extensions.Telephony;

    internal sealed class TelephoneNumberRenderer : AttributeRenderer
    {
        public TelephoneNumberRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            return this.ToTex(((TelephoneNumber)obj).ToString(culture));
        }
    }
}
