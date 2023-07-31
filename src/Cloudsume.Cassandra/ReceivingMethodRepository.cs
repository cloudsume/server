namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Financial;
    using Cornot;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    internal sealed class ReceivingMethodRepository : IReceivingMethodRepository
    {
        private readonly IMapperFactory db;
        private readonly IReadConsistencyProvider readConsistencies;

        public ReceivingMethodRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
        {
            this.db = db;
            this.readConsistencies = readConsistencies;
        }

        public Task CreateAsync(ReceivingMethod method, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.PaymentReceivingMethod()
            {
                UserId = method.UserId,
                Id = method.Id,
                CreatedAt = method.CreatedAt,
            };

            switch (method)
            {
                case Cloudsume.Stripe.ReceivingMethod m:
                    row.StripeAccount = m.AccountId;
                    break;
                default:
                    throw new ArgumentException($"Unknown method {method.GetType()}.", nameof(method));
            }

            return db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
        }

        public async Task<ReceivingMethod?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE user = ? AND id = ?", userId, id).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var row = await db.FirstOrDefaultAsync<Models.PaymentReceivingMethod>(query);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task<IEnumerable<ReceivingMethod>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE user = ?", userId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var rows = await db.FetchAsync<Models.PaymentReceivingMethod>(query);

            return rows.Select(this.ToDomain).ToArray();
        }

        private ReceivingMethod ToDomain(Models.PaymentReceivingMethod row)
        {
            if (row.StripeAccount != null)
            {
                return new Cloudsume.Stripe.ReceivingMethod(row.Id, row.UserId, row.StripeAccount, row.CreatedAt.LocalDateTime);
            }
            else
            {
                throw new DataCorruptionException(row, "Invalid data.");
            }
        }
    }
}
