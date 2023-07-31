namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class TelephoneProperty : DataProperty<TelephoneProperty, Telephone?>
    {
        public static readonly UdtMap Mapping = CreateMapping("telprop");
    }
}
