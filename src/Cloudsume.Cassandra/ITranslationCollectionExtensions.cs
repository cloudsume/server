namespace Cloudsume.Cassandra
{
    using System.Collections.Generic;
    using System.Linq;
    using Candidate.Globalization;

    internal static class ITranslationCollectionExtensions
    {
        public static IDictionary<string, string> ToCassandra(this ITranslationCollection translations)
        {
            return translations.ToDictionary(n => n.Key.Name, n => n.Value);
        }
    }
}
