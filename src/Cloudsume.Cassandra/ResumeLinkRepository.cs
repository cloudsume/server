namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;
    using Cornot;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    internal sealed class ResumeLinkRepository : IResumeLinkRepository
    {
        private readonly IMapperFactory db;
        private readonly IReadConsistencyProvider readConsistencies;

        public ResumeLinkRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
        {
            this.db = db;
            this.readConsistencies = readConsistencies;
        }

        public async Task CreateAsync(Link link, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.ResumeLink()
            {
                ResumeId = link.Resume,
                Id = link.Id.Value,
                Name = link.Name,
                UserId = link.User,
                Censorships = link.Censorships.Select(c => c.ToString()).ToArray(),
                CreatedAt = link.CreateAt.ToUniversalTime(),
            };

            await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
        }

        public async Task<Link?> FindAsync(LinkId id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("FROM resume_links_by_id WHERE id = ?", id.Value).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var row = await db.FirstOrDefaultAsync<Models.ResumeLink>(query);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<Link?> GetAsync(Guid resumeId, LinkId linkId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE resume_id = ? AND id = ?", resumeId, linkId.Value).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var row = await db.FirstOrDefaultAsync<Models.ResumeLink>(query);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<IEnumerable<Link>> ListAsync(Guid resumeId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE resume_id = ?", resumeId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var rows = await db.FetchAsync<Models.ResumeLink>(query);

            return rows.Select(this.ToDomain).OrderBy(l => l.CreateAt).ToArray();
        }

        public Task SetCensorshipsAsync(Guid resumeId, LinkId linkId, IReadOnlySet<LinkCensorship> censorships, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql
                .New("SET censorships = ? WHERE resume_id = ? AND id = ?", censorships.Select(c => c.ToString()).ToArray(), resumeId, linkId.Value)
                .WithOptions(options => options.SetConsistencyLevel(ConsistencyLevel.EachQuorum));

            return db.UpdateAsync<Models.ResumeLink>(query);
        }

        public async Task DeleteAsync(Guid resumeId, LinkId id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var delete = Cql.New("WHERE resume_id = ? AND id = ?", resumeId, id.Value).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
            });

            await db.DeleteAsync<Models.ResumeLink>(delete);
        }

        public async Task<int> CountAsync(Guid resumeId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();

            return await db.SingleAsync<int>("SELECT COUNT(*) FROM resume_links WHERE resume_id = ?", resumeId);
        }

        public async Task TransferAsync(Guid resumeId, Guid to, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE resume_id = ?", resumeId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            foreach (var link in await db.FetchAsync<Models.ResumeLink>(query))
            {
                link.UserId = to;

                await db.UpdateAsync(link, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
            }
        }

        private Link ToDomain(Models.ResumeLink row)
        {
            var id = new LinkId(row.Id);

            if (row.Name is not { } name)
            {
                throw new DataCorruptionException(row, "No resume name.");
            }

            if (row.UserId is not { } userId)
            {
                throw new DataCorruptionException(row, "No owner for current resume.");
            }

            var resumeId = row.ResumeId;
            var censorships = row.Censorships?.Select(id => LinkCensorship.Parse(id)).ToHashSet() ?? new();
            var createdAt = row.CreatedAt.LocalDateTime;

            return new(id, name, userId, resumeId, censorships, createdAt);
        }
    }
}
