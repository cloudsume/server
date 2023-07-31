namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class SByteProperty : DataProperty<SByteProperty, sbyte?>
    {
        public static readonly UdtMap Mapping = CreateMapping("tinyintprop");
    }
}
