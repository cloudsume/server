namespace System.Text
{
    using System.Globalization;

    public static class RuneExtensions
    {
        // Thai.
        private static readonly CultureInfo Thai = CultureInfo.GetCultureInfo("th");
        private static readonly Rune ThaiStart = new(0x0E00);
        private static readonly Rune ThaiEnd = new(0x0E7F);

        public static CultureInfo GetCulture(this Rune rune)
        {
            if (rune >= ThaiStart && rune <= ThaiEnd)
            {
                return Thai;
            }
            else
            {
                return CultureInfo.InvariantCulture;
            }
        }
    }
}
