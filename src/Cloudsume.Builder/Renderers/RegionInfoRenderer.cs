namespace Cloudsume.Builder.Renderers
{
    using System;
    using System.Globalization;
    using Candidate.Globalization;
    using Cloudsume.Builder;

    internal sealed class RegionInfoRenderer : AttributeRenderer
    {
        public RegionInfoRenderer(CultureInfo templateCulture)
            : base(templateCulture)
        {
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var name = RegionNames.FindByCode(((RegionInfo)obj).Name, culture);

            if (name == null)
            {
                throw new ArgumentException("Unsupported region.", nameof(obj));
            }

            return this.ToTex(name);
        }
    }
}
