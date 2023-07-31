namespace Cloudsume.Server.Cassandra
{
    using System;
    using global::Cassandra;
    using Ultima.Extensions.Primitives;

    internal static class LocalDateExtensions
    {
        public static YearMonth ToDateMonth(this LocalDate v) => new(v.Year, v.Month);

        public static DateTime ToUtc(this LocalDate v) => new(v.Year, v.Month, v.Day, 0, 0, 0, DateTimeKind.Utc);
    }
}
