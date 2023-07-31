namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Financial;
    using Cloudsume.Resume;
    using Cloudsume.Template;
    using global::Cassandra;
    using global::Cassandra.Mapping;
    using NetUlid;

    internal sealed class TemplateLicenseRepository : ITemplateLicenseRepository
    {
        private readonly IMapperFactory db;
        private readonly IReadConsistencyProvider readConsistencies;

        public TemplateLicenseRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
        {
            this.db = db;
            this.readConsistencies = readConsistencies;
        }

        public Task CreateAsync(TemplateLicense license, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.TemplateLicense()
            {
                RegistrationId = license.RegistrationId,
                Id = license.Id.ToByteArray(),
                UserId = license.UserId,
                Status = Convert.ToSByte(license.Status),
                UpdatedAt = license.UpdatedAt,
            };

            switch (license.Payment)
            {
                case Cloudsume.Stripe.PaymentMethod m:
                    row.StripePaymentIntent = m.Id;
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentException($"Unknown payment method {license.Payment.GetType()}.", nameof(license));
            }

            return db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
        }

        public Task DeleteAsync(Guid registrationId, Ulid id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var cql = Cql.New("WHERE registration_id = ? AND id = ?", registrationId, id.ToByteArray()).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
            });

            return db.DeleteAsync<Models.TemplateLicense>(cql);
        }

        public async Task<TemplateLicense?> GetAsync(Guid userId, Guid registrationId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("FROM template_licenses_by_user_id WHERE user_id = ? AND registration_id = ?", userId, registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var row = await db.FirstOrDefaultAsync<Models.TemplateLicense>(query);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<IEnumerable<TemplateLicense>> ListByRegistrationAsync(Guid registrationId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var rows = await db.FetchAsync<Models.TemplateLicense>("WHERE registration_id = ?", registrationId);

            return rows.Select(this.ToDomain).ToArray();
        }

        public async Task<IEnumerable<TemplateLicense>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("FROM template_licenses_by_user_id WHERE user_id = ?", userId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var rows = await db.FetchAsync<Models.TemplateLicense>(query);

            return rows.Select(this.ToDomain).ToArray();
        }

        public Task SetPaymentAsync(Guid registrationId, Ulid id, PaymentMethod? payment, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();

            // Create update statements.
            var arguments = new List<object>();
            string update;

            switch (payment)
            {
                case Cloudsume.Stripe.PaymentMethod m:
                    update = "SET stripe_payment_intent = ?";
                    arguments.Add(m.Id);
                    break;
                case null:
                    update = "SET stripe_payment_intent = null";
                    break;
                default:
                    throw new ArgumentException($"Unknown payment method {payment.GetType()}.", nameof(payment));
            }

            // Execute update.
            arguments.Add(registrationId);
            arguments.Add(id.ToByteArray());

            return db.UpdateAsync<Models.TemplateLicense>(Cql.New($"{update} WHERE registration_id = ? AND id = ?", arguments.ToArray()).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
            }));
        }

        public Task SetStatusAsync(Guid registrationId, Ulid id, TemplateLicenseStatus status, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var cql = Cql.New("SET status = ? WHERE registration_id = ? AND id = ?", Convert.ToSByte(status), registrationId, id.ToByteArray());

            return db.UpdateAsync<Models.TemplateLicense>(cql.WithOptions(options => options.SetConsistencyLevel(ConsistencyLevel.EachQuorum)));
        }

        public Task SetUpdatedAsync(Guid registrationId, Ulid id, DateTime updatedAt, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var cql = Cql.New("SET updated_at = ? WHERE registration_id = ? AND id = ?", new DateTimeOffset(updatedAt), registrationId, id.ToByteArray());

            return db.UpdateAsync<Models.TemplateLicense>(cql);
        }

        public async Task TransferAsync(Guid from, Guid to, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("FROM template_licenses_by_user_id WHERE user_id = ?", from).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            foreach (var license in await db.FetchAsync<Models.TemplateLicense>(query))
            {
                if (license.Status != Convert.ToSByte(TemplateLicenseStatus.Valid))
                {
                    continue;
                }

                license.UserId = to;

                await db.UpdateAsync(license, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
            }
        }

        private TemplateLicense ToDomain(Models.TemplateLicense row)
        {
            PaymentMethod? payment = null;

            if (row.StripePaymentIntent != null)
            {
                payment = new Cloudsume.Stripe.PaymentMethod(row.StripePaymentIntent);
            }

            return new(new(row.Id), row.RegistrationId, row.UserId, payment, (TemplateLicenseStatus)row.Status, row.UpdatedAt.LocalDateTime);
        }
    }
}
