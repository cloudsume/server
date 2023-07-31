namespace Cloudsume.Cassandra.Models
{
    using System;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class TemplateLicense
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateLicense>("template_licenses")
            .Column(l => l.RegistrationId, c => c.WithName("registration_id"))
            .Column(l => l.Id, c => c.WithName("id"))
            .Column(l => l.UserId, c => c.WithName("user_id"))
            .Column(l => l.StripePaymentIntent, c => c.WithName("stripe_payment_intent"))
            .Column(l => l.Status, c => c.WithName("status"))
            .Column(l => l.UpdatedAt, c => c.WithName("updated_at"))
            .PartitionKey(l => l.RegistrationId)
            .ClusteringKey(l => l.Id, SortOrder.Descending);

        public Guid RegistrationId { get; set; }

        public byte[] Id { get; set; } = Array.Empty<byte>();

        public Guid UserId { get; set; }

        public string? StripePaymentIntent { get; set; }

        public sbyte Status { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
