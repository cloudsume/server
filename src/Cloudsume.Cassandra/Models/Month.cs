namespace Cloudsume.Cassandra.Models
{
    using System;
    using global::Cassandra;
    using Ultima.Extensions.Primitives;

    public sealed class Month
    {
        public static readonly UdtMap Mapping = UdtMap.For<Month>("month")
            .Map(m => m.Year, "year")
            .Map(m => m.Value, "month");

        public short Year { get; set; }

        public sbyte Value { get; set; }

        public static Month? From(YearMonth? value) => value is { } v
            ? new() { Year = Convert.ToInt16(v.Year), Value = Convert.ToSByte(v.Month) }
            : null;

        public static Month? From(DateTime? value) => value is { } v
            ? new() { Year = Convert.ToInt16(v.Year), Value = Convert.ToSByte(v.Month) }
            : null;

        public YearMonth ToDateMonth() => new(this.Year, this.Value);

        public DateTime ToUtc() => new(this.Year, this.Value, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
