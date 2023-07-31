namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;
    using global::Cassandra.Mapping;
    using NetUlid;

    internal sealed class ResumeLinkAccessRepository : ILinkAccessRepository
    {
        private readonly IMapperFactory db;

        public ResumeLinkAccessRepository(IMapperFactory db)
        {
            this.db = db;
        }

        public async Task CreateAsync(LinkId linkId, IPAddress? from, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.ResumeLinkAccess()
            {
                LinkId = linkId.Value,
                Id = Ulid.Generate().ToByteArray(),
                From = from,
            };

            await db.InsertAsync(row);
        }

        public Task DeleteAsync(LinkId link, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();

            return db.DeleteAsync<Models.ResumeLinkAccess>("WHERE link_id = ?", link.Value);
        }

        public async Task<LinkAccess?> GetLatestAsync(LinkId link, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = await db.FirstOrDefaultAsync<Models.ResumeLinkAccess>("WHERE link_id = ? ORDER BY id DESC LIMIT 1", link.Value).ConfigureAwait(false);

            if (row is null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<IEnumerable<LinkAccess>> ListAsync(LinkId linkId, int limit, Ulid? skipTill, CancellationToken cancellationToken = default)
        {
            if (limit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            var db = this.db.CreateMapper();
            Cql query;

            if (skipTill is { } s)
            {
                query = Cql.New("WHERE link_id = ? AND id < ? ORDER BY id DESC LIMIT ?", linkId.Value, s.ToByteArray(), limit);
            }
            else
            {
                query = Cql.New("WHERE link_id = ? ORDER BY id DESC LIMIT ?", linkId.Value, limit);
            }

            var accesses = await db.FetchAsync<Models.ResumeLinkAccess>(query);

            return accesses.Select(this.ToDomain).ToArray();
        }

        private LinkAccess ToDomain(Models.ResumeLinkAccess row)
        {
            var id = new Ulid(row.Id);

            return new LinkAccess(id, row.From);
        }
    }
}
