namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class IeltsScore
    {
        public static readonly UdtMap Mapping = UdtMap.For<IeltsScore>("ielts")
            .Map(s => s.Type, "type")
            .Map(s => s.Band, "band");

        public sbyte Type { get; set; }

        public decimal Band { get; set; }
    }
}
