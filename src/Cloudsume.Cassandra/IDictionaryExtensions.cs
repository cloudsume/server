namespace Cloudsume.Cassandra
{
    using System.Collections.Generic;
    using System.Linq;
    using Ultima.Extensions.Currency;

    internal static class IDictionaryExtensions
    {
        public static Dictionary<CurrencyCode, decimal> ToPriceList(this IDictionary<string, decimal> column)
        {
            return column.ToDictionary(i => CurrencyCode.Parse(i.Key), i => i.Value);
        }
    }
}
