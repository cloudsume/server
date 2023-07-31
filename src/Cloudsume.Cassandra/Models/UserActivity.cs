namespace Cloudsume.Cassandra.Models;

using System;
using System.Net;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class UserActivity
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<UserActivity>("user_activities")
        .Column(a => a.UserId, c => c.WithName("user"))
        .Column(a => a.Id, c => c.WithName("id"))
        .Column(a => a.Type, c => c.WithName("type"))
        .Column(a => a.Data, c => c.WithName("data"))
        .Column(a => a.IpAddress, c => c.WithName("ip"))
        .Column(a => a.UserAgent, c => c.WithName("user_agent"))
        .PartitionKey(a => a.UserId)
        .ClusteringKey(a => a.Id, SortOrder.Descending);

    public Guid UserId { get; set; }

    public byte[] Id { get; set; } = Array.Empty<byte>();

    public Guid Type { get; set; }

    public byte[]? Data { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}
