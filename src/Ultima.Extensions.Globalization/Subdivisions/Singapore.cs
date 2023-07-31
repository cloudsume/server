namespace Ultima.Extensions.Globalization.Subdivisions
{
    using System.Collections.Generic;

    internal static class Singapore
    {
        public static readonly Dictionary<string, string> CentralSingapore = new()
        {
            { "en", "Central Singapore" },
            { "th", "ภาคกลาง" },
        };

        public static readonly Dictionary<string, string> NorthEast = new()
        {
            { "en", "North East" },
            { "th", "ภาคตะวันออกเฉียงเหนือ" },
        };

        public static readonly Dictionary<string, string> NorthWest = new()
        {
            { "en", "North West" },
            { "th", "ภาคตะวันตกเฉียงเหนือ" },
        };

        public static readonly Dictionary<string, string> SouthEast = new()
        {
            { "en", "South East" },
            { "th", "ภาคตะวันออกเฉียงใต้" },
        };

        public static readonly Dictionary<string, string> SouthWest = new()
        {
            { "en", "South West" },
            { "th", "ภาคตะวันตกเฉียงใต้" },
        };
    }
}
