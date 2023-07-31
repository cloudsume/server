namespace Cloudsume.Cassandra.Models;

using System;
using System.Collections.Generic;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplateRegistration
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateRegistration>("template_registrations")
        .Column(r => r.Id, c => c.WithName("id"))
        .Column(r => r.OwnerId, c => c.WithName("owner_id"))
        .Column(r => r.Name, c => c.WithName("name"))
        .Column(r => r.Description, c => c.WithName("description"))
        .Column(r => r.Website, c => c.WithName("website"))
        .Column(r => r.Language, c => c.WithName("language"))
        .Column(r => r.ApplicableJobs, c => c.WithName("applicable_jobs").AsFrozen())
        .Column(r => r.Category, c => c.WithName("category"))
        .Column(r => r.Prices, c => c.WithName("prices"))
        .Column(r => r.UnlistedReason, c => c.WithName("unlisted"))
        .Column(r => r.CreatedAt, c => c.WithName("created_at"))
        .Column(r => r.UpdatedAt, c => c.WithName("updated_at"))
        .PartitionKey(r => r.Id);

    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Language { get; set; }

    public IEnumerable<Guid>? ApplicableJobs { get; set; }

    public sbyte Category { get; set; }

    public IDictionary<string, decimal>? Prices { get; set; }

    public sbyte? UnlistedReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
