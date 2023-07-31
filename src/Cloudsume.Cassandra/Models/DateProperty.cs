namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class DateProperty : DataProperty<DateProperty, LocalDate?>
    {
        public static readonly UdtMap Mapping = CreateMapping("dateprop");
    }
}
