namespace Cloudsume.Cassandra.Models;

using System;
using System.Net;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class Feedback
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<Feedback>("feedbacks")
        .Column(f => f.Id, c => c.WithName("id"))
        .Column(f => f.Score, c => c.WithName("score"))
        .Column(f => f.Detail, c => c.WithName("detail"))
        .Column(f => f.Contact, c => c.WithName("contact"))
        .Column(f => f.User, c => c.WithName("user"))
        .Column(f => f.IpAddress, c => c.WithName("ip"))
        .Column(f => f.UserAgent, c => c.WithName("user_agent"))
        .PartitionKey(f => f.Id);

    public byte[] Id { get; set; } = Array.Empty<byte>();

    public sbyte Score { get; set; }

    public string? Detail { get; set; }

    public string? Contact { get; set; }

    public Guid? User { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}
