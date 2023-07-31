namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Globalization;
    using Cloudsume.Data;
    using Cornot;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public sealed class JobRepository : IJobRepository
    {
        private readonly IMapperFactory db;

        public JobRepository(IMapperFactory db)
        {
            this.db = db;
        }

        public async Task CreateAsync(Job job, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.Job()
            {
                Id = job.Id,
                Names = job.Names.ToCassandra(),
            };

            await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.All));
        }

        public async Task<Job?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = await db.FirstOrDefaultAsync<Models.Job>("WHERE id = ?", id);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();

            return await db.FirstOrDefaultAsync<Guid?>("SELECT id FROM jobs WHERE id = ?", id) != null;
        }

        public async Task<IEnumerable<Job>> ListAsync(CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var rows = await db.FetchAsync<Models.Job>();

            return rows.Select(this.ToDomain).ToArray();
        }

        private Job ToDomain(Models.Job row)
        {
            if (row.Names == null)
            {
                throw new DataCorruptionException(row, $"{nameof(row.Names)} is null.");
            }

            return new(row.Id, new TranslationCollection(row.Names));
        }
    }
}
