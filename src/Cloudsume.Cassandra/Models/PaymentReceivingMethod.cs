namespace Cloudsume.Cassandra.Models
{
    using System;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class PaymentReceivingMethod
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<PaymentReceivingMethod>("payment_receiving_methods")
            .Column(p => p.UserId, c => c.WithName("user"))
            .Column(p => p.Id, c => c.WithName("id"))
            .Column(p => p.StripeAccount, c => c.WithName("stripe_account"))
            .Column(p => p.CreatedAt, c => c.WithName("created_at"))
            .PartitionKey(p => p.UserId)
            .ClusteringKey(p => p.Id);

        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public string? StripeAccount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
