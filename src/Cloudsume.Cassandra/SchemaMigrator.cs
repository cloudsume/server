namespace Cloudsume.Cassandra
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using CassandraMigrator;
    using CassandraMigrator.DataStaxClient;
    using CassandraMigrator.Provider;
    using global::Cassandra;
    using Microsoft.Extensions.Logging;

    internal sealed class SchemaMigrator : ISchemaMigrator
    {
        private readonly ILoggerFactory logger;
        private readonly ISession cassandra;

        public SchemaMigrator(ILoggerFactory logger, ISession cassandra)
        {
            this.logger = logger;
            this.cassandra = cassandra;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var connection = await Connection.CreateAsync(this.cassandra, cancellationToken);

            var migrator = new Migrator(connection, this.logger.CreateLogger<Migrator>());
            var provider = new FileSystemMigrationProvider(Path.Join(AppContext.BaseDirectory, "cassandra", "schemas"));

            await migrator.ExecuteAsync(provider, cancellationToken);
        }
    }
}
