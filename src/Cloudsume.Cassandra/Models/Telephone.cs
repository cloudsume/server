namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;
    using Domain = Ultima.Extensions.Telephony.TelephoneNumber;

    public sealed class Telephone
    {
        public static readonly UdtMap Mapping = UdtMap.For<Telephone>("telephone")
            .Map(t => t.Country, "country")
            .Map(t => t.Number, "number");

        public string? Country { get; set; }

        public string? Number { get; set; }

        public static Telephone? From(Domain? v) => (v == null) ? null : new() { Country = v.Country.Name, Number = v.Number };
    }
}
