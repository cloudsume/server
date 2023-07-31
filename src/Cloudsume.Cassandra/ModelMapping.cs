namespace Cloudsume.Server.Cassandra
{
    using global::Cassandra.Mapping;

    internal static class ModelMapping
    {
        public static Map<T> Create<T>(string table) => new Map<T>()
            .TableName(table)
            .ExplicitColumns();
    }
}
