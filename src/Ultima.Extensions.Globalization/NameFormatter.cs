namespace Candidate.Globalization
{
    using System;
    using System.Globalization;

    public abstract class NameFormatter
    {
        public static NameFormatter GetForRegion(RegionInfo region)
        {
            switch (region.Name)
            {
                case "TH":
                case "US":
                    return NameFormatters.Western.Default;
                default:
                    throw new ArgumentException($"Unsupported region {region.Name}.", nameof(region));
            }
        }

        public string Format(string first, string last)
        {
            return this.Format(first, null, last);
        }

        public abstract string Format(string first, string? middle, string last);
    }
}
