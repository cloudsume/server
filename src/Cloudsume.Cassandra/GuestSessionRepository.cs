namespace Cloudsume.Cassandra;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Identity;
using Cornot;
using global::Cassandra;
using global::Cassandra.Mapping;

internal sealed class GuestSessionRepository : IGuestSessionRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;

    public GuestSessionRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
    }

    public Task CreateAsync(GuestSession session, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var dto = new Models.GuestSession()
        {
            UserId = session.UserId,
            Issuer = session.Issuer,
            KeyId = session.KeyId,
            Requester = session.Requester,
            CreatedAt = session.CreatedAt,
            TransferredTo = session.TransferredTo,
            TransferredAt = session.TransferredAt,
        };

        return db.InsertAsync(dto, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
    }

    public async Task<GuestSession?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE user_id = ?", id).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        var row = await db.FirstOrDefaultAsync<Models.GuestSession>(query);

        if (row == null)
        {
            return null;
        }

        return this.ToDomain(row);
    }

    public async Task SetTransferredAsync(Guid id, Guid to, DateTime time, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("SET transferred_to = ?, transferred_at = ? WHERE user_id = ?", to, new DateTimeOffset(time), id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        });

        await db.UpdateAsync<Models.GuestSession>(query);
    }

    private GuestSession ToDomain(Models.GuestSession dto)
    {
        var issuer = dto.Issuer;

        if (issuer == null)
        {
            throw new DataCorruptionException(dto, $"{nameof(dto.Issuer)} is null.");
        }

        var keyId = dto.KeyId;

        if (keyId == null)
        {
            throw new DataCorruptionException(dto, $"{nameof(dto.KeyId)} is null.");
        }

        var requester = dto.Requester;

        if (requester == null)
        {
            throw new DataCorruptionException(dto, $"{nameof(dto.Requester)} is null.");
        }

        return new(dto.UserId, issuer, keyId, requester, dto.CreatedAt.LocalDateTime, dto.TransferredTo, dto.TransferredAt?.LocalDateTime);
    }
}
