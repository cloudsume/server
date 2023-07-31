namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Financial;
using Cloudsume.Template;
using global::Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplatePackLicenseRepository : ITemplatePackLicenseRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;

    public TemplatePackLicenseRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
    }

    public Task CreateAsync(TemplatePackLicense license, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var row = new Models.TemplatePackLicense()
        {
            PackId = license.PackId,
            Id = license.Id.ToByteArray(),
            UserId = license.UserId,
            Status = Convert.ToSByte(license.Status),
            UpdatedAt = license.UpdatedAt,
        };

        switch (license.Payment)
        {
            case null:
                break;
            case Cloudsume.Stripe.PaymentMethod m:
                row.StripePaymentIntent = m.Id;
                break;
            default:
                throw new ArgumentException($"Unknown payment {license.Payment.GetType()}.", nameof(license));
        }

        return db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
    }

    public async Task<TemplatePackLicense?> GetAsync(Guid userId, Guid packId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("FROM template_pack_licenses_by_user WHERE user = ? AND pack = ?", userId, packId).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        var row = await db.FirstOrDefaultAsync<Models.TemplatePackLicense>(query);

        if (row is null)
        {
            return null;
        }

        return this.ToDomain(row);
    }

    public async Task<IEnumerable<TemplatePackLicense>> ListAsync(Guid packId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE pack = ?", packId).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var rows = await db.FetchAsync<Models.TemplatePackLicense>(query);

        return rows.Select(this.ToDomain).ToArray();
    }

    private TemplatePackLicense ToDomain(Models.TemplatePackLicense row)
    {
        PaymentMethod? payment;

        if (row.StripePaymentIntent is not null)
        {
            payment = new Cloudsume.Stripe.PaymentMethod(row.StripePaymentIntent);
        }
        else
        {
            payment = null;
        }

        return new(new(row.Id), row.PackId, row.UserId, payment, (TemplateLicenseStatus)row.Status, row.UpdatedAt.LocalDateTime);
    }
}
