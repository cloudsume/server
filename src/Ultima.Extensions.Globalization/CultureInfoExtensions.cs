namespace Candidate.Globalization
{
    using System.Globalization;

    public static class CultureInfoExtensions
    {
        public static bool ExistsInTree(this CultureInfo culture, CultureInfo top)
        {
            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                return true;
            }

            for (var current = top; !current.Equals(CultureInfo.InvariantCulture); current = current.Parent)
            {
                if (culture.Equals(current))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
