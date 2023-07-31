namespace Cloudsume.Cassandra.Models
{
    using System;
    using global::Cassandra;

    public sealed class UuidProperty : DataProperty<UuidProperty, Guid?>
    {
        public static readonly UdtMap Mapping = CreateMapping("uuidprop");
    }
}
