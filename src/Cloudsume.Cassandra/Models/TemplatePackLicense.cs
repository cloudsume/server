namespace Cloudsume.Cassandra.Models;

using System;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplatePackLicense
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplatePackLicense>("template_pack_licenses")
        .Column(l => l.PackId, c => c.WithName("pack"))
        .Column(l => l.Id, c => c.WithName("id"))
        .Column(l => l.UserId, c => c.WithName("user"))
        .Column(l => l.StripePaymentIntent, c => c.WithName("stripe_payment_intent"))
        .Column(l => l.Status, c => c.WithName("status"))
        .Column(l => l.UpdatedAt, c => c.WithName("updated"))
        .PartitionKey(l => l.PackId)
        .ClusteringKey(l => l.Id, SortOrder.Descending);

    public Guid PackId { get; set; }

    public byte[] Id { get; set; } = Array.Empty<byte>();

    public Guid UserId { get; set; }

    public string? StripePaymentIntent { get; set; }

    public sbyte Status { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
