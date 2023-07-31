namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class MonthProperty : DataProperty<MonthProperty, Month?>
    {
        public static readonly UdtMap Mapping = CreateMapping("monthprop");
    }
}
