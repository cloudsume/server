namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class ShortProperty : DataProperty<ShortProperty, short?>
    {
        public static readonly UdtMap Mapping = CreateMapping("smallintprop");
    }
}
