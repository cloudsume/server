namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Template;
using Cornot;
using global::Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplatePackRepository : ITemplatePackRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;

    public TemplatePackRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
    }

    public async Task CreateAsync(TemplatePack pack, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Insert pack.
        var row = new Models.TemplatePack()
        {
            Id = pack.Id,
            OwnerId = pack.UserId,
            Name = pack.Name.NullOnEmpty(),
            Prices = pack.Prices.ToCassandra(),
            CreatedAt = pack.CreatedAt,
            UpdatedAt = pack.UpdatedAt,
        };

        await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));

        // Insert member.
        foreach (var registrationId in pack.Templates)
        {
            var member = new Models.TemplatePackMember()
            {
                PackId = pack.Id,
                TemplateId = registrationId,
            };

            await db.InsertAsync(member, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
        }
    }

    public async Task<TemplatePack?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE id = ?", id).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var row = await db.FirstOrDefaultAsync<Models.TemplatePack>(query);

        if (row is null)
        {
            return null;
        }

        return await this.ToDomainAsync(db, row);
    }

    public async Task<IEnumerable<TemplatePack>> GetPacksAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        // Fetch pack IDs.
        var db = this.db.CreateMapper();
        var domains = new List<TemplatePack>();
        var query = Cql.New("FROM template_pack_members_by_template WHERE template = ?", registrationId).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        foreach (var member in await db.FetchAsync<Models.TemplatePackMember>(query))
        {
            // Fetch pack.
            query = Cql.New("WHERE id = ?", member.PackId).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
            var row = await db.FirstOrDefaultAsync<Models.TemplatePack>(query);

            if (row is null)
            {
                throw new DataCorruptionException(member, $"{nameof(member.PackId)} does not exists.");
            }

            domains.Add(await this.ToDomainAsync(db, row));
        }

        return domains;
    }

    public async Task<IEnumerable<TemplatePack>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<TemplatePack>();
        var query = Cql.New("FROM template_packs_by_owner WHERE owner = ?", userId).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        foreach (var row in await db.FetchAsync<Models.TemplatePack>(query))
        {
            domains.Add(await this.ToDomainAsync(db, row));
        }

        return domains;
    }

    public async Task<IEnumerable<TemplatePack>> ListAsync(CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<TemplatePack>();
        var options = CqlQueryOptions.New().SetConsistencyLevel(this.readConsistencies.StrongConsistency);

        foreach (var row in await db.FetchAsync<Models.TemplatePack>(options))
        {
            domains.Add(await this.ToDomainAsync(db, row));
        }

        return domains;
    }

    private async Task<TemplatePack> ToDomainAsync(IMapper db, Models.TemplatePack row)
    {
        var templates = new HashSet<Guid>();
        var query = Cql.New("WHERE pack = ?", row.Id).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));

        foreach (var member in await db.FetchAsync<Models.TemplatePackMember>(query))
        {
            templates.Add(member.TemplateId);
        }

        return new(
            row.Id,
            row.OwnerId,
            row.Name ?? string.Empty,
            row.Prices?.ToPriceList() ?? new(),
            templates,
            row.CreatedAt.LocalDateTime,
            row.UpdatedAt.LocalDateTime);
    }
}
