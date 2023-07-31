namespace Cloudsume.Cassandra.Models
{
    using System;
    using global::Cassandra;

    public abstract class DataProperty<TModel, TValue> where TModel : DataProperty<TModel, TValue>, new()
    {
        public sbyte Flags { get; set; }

        public TValue? Value { get; set; }

        public static TModel From(Cloudsume.Resume.DataProperty<TValue> property) => new()
        {
            Flags = Convert.ToSByte(property.Flags),
            Value = property.Value,
        };

        public static TModel From<T>(Cloudsume.Resume.DataProperty<T> property, Func<T?, TValue> getter) => new()
        {
            Flags = Convert.ToSByte(property.Flags),
            Value = getter(property.Value),
        };

        protected static UdtMap CreateMapping(string name) => UdtMap.For<TModel>(name)
            .Map(p => p.Flags, "flags")
            .Map(p => p.Value, "value");
    }
}
