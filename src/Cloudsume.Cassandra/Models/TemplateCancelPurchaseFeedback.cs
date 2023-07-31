namespace Cloudsume.Cassandra.Models;

using System;
using System.Net;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplateCancelPurchaseFeedback
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateCancelPurchaseFeedback>("template_cancel_purchase_feedbacks")
        .Column(f => f.TemplateId, c => c.WithName("template"))
        .Column(f => f.Id, c => c.WithName("id"))
        .Column(f => f.UserId, c => c.WithName("user"))
        .Column(f => f.StripePaymentIntent, c => c.WithName("stripe_payment_intent"))
        .Column(f => f.Reason, c => c.WithName("reason"))
        .Column(f => f.Ip, c => c.WithName("ip"))
        .Column(f => f.UserAgent, c => c.WithName("user_agent"))
        .PartitionKey(f => f.TemplateId)
        .ClusteringKey(f => f.Id, SortOrder.Descending);

    public Guid TemplateId { get; set; }

    public byte[] Id { get; set; } = Array.Empty<byte>();

    public Guid UserId { get; set; }

    public string? StripePaymentIntent { get; set; }

    public sbyte Reason { get; set; }

    public IPAddress? Ip { get; set; }

    public string? UserAgent { get; set; }
}
