namespace Cloudsume.Cassandra;

using global::Cassandra;

public interface IReadConsistencyProvider
{
    /// <summary>
    /// Gets a consistency level to achieve strong consistency in the local quorum.
    /// </summary>
    ConsistencyLevel StrongConsistency { get; }
}
