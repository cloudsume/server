namespace Cloudsume.Cassandra;

using global::Cassandra;
using Microsoft.Extensions.Options;

internal sealed class ReadConsistencyProvider : IReadConsistencyProvider
{
    private readonly CassandraOptions options;

    public ReadConsistencyProvider(IOptions<CassandraOptions> options)
    {
        this.options = options.Value;
    }

    public ConsistencyLevel StrongConsistency => this.options.ReadConsistencies.StrongConsistency;
}
