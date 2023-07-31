namespace Cloudsume.Cassandra
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISchemaMigrator
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
