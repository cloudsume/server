namespace Cloudsume.Cassandra.Models;

using System;
using System.Net;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class GuestSession
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<GuestSession>("guest_sessions")
        .Column(s => s.UserId, c => c.WithName("user_id"))
        .Column(s => s.Issuer, c => c.WithName("issuer"))
        .Column(s => s.KeyId, c => c.WithName("key_id"))
        .Column(s => s.Requester, c => c.WithName("requester"))
        .Column(s => s.CreatedAt, c => c.WithName("created_at"))
        .Column(s => s.TransferredTo, c => c.WithName("transferred_to"))
        .Column(s => s.TransferredAt, c => c.WithName("transferred_at"))
        .PartitionKey(s => s.UserId);

    public Guid UserId { get; set; }

    public string? Issuer { get; set; }

    public string? KeyId { get; set; }

    public IPAddress? Requester { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid? TransferredTo { get; set; }

    public DateTimeOffset? TransferredAt { get; set; }
}
