namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class IeltsScoreProperty : DataProperty<IeltsScoreProperty, IeltsScore?>
    {
        public static readonly UdtMap Mapping = CreateMapping("ieltsprop");
    }
}
