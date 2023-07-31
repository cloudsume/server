namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class TextProperty : DataProperty<TextProperty, string?>
    {
        public static readonly UdtMap Mapping = CreateMapping("textprop");
    }
}
