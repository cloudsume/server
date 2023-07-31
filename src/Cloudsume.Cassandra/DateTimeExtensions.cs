namespace Cloudsume.Cassandra;

using System;
using global::Cassandra;

internal static class DateTimeExtensions
{
    public static LocalDate ToLocalDate(this DateTime from)
    {
        var utc = from.ToUniversalTime();
        return new(utc.Year, utc.Month, utc.Day);
    }
}
