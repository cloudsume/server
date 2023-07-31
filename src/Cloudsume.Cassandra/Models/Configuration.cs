namespace Cloudsume.Cassandra.Models;

using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class Configuration
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<Configuration>("configurations")
        .Column(c => c.Name, c => c.WithName("name"))
        .Column(c => c.Value, c => c.WithName("value"))
        .PartitionKey(c => c.Name);

    public string? Name { get; set; }

    public byte[]? Value { get; set; }
}
