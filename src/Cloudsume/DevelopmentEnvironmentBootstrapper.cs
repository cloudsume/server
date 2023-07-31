namespace Cloudsume
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Cassandra;
    using Microsoft.Extensions.Hosting;

    internal sealed class DevelopmentEnvironmentBootstrapper : IHostedService
    {
        private readonly ISchemaMigrator cassandra;

        public DevelopmentEnvironmentBootstrapper(ISchemaMigrator cassandra)
        {
            this.cassandra = cassandra;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.MigrateCassandraAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task MigrateCassandraAsync(CancellationToken cancellationToken = default)
        {
            await this.cassandra.ExecuteAsync(cancellationToken);
        }
    }
}
