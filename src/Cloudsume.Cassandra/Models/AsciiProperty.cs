namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class AsciiProperty : DataProperty<AsciiProperty, string?>
    {
        public static readonly UdtMap Mapping = CreateMapping("asciiprop");
    }
}
