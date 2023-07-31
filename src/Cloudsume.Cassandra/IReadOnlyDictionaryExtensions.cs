namespace Cloudsume.Cassandra
{
    using System.Collections.Generic;
    using System.Linq;
    using Ultima.Extensions.Currency;

    internal static class IReadOnlyDictionaryExtensions
    {
        public static IDictionary<string, decimal> ToCassandra(this IReadOnlyDictionary<CurrencyCode, decimal> prices)
        {
            return prices.ToDictionary(i => i.Key.Value, i => i.Value);
        }
    }
}
