namespace Cloudsume.Cassandra.Models;

using System;
using System.Collections.Generic;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplatePack
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplatePack>("template_packs")
        .Column(p => p.Id, c => c.WithName("id"))
        .Column(p => p.OwnerId, c => c.WithName("owner"))
        .Column(p => p.Name, c => c.WithName("name"))
        .Column(p => p.Prices, c => c.WithName("prices"))
        .Column(p => p.CreatedAt, c => c.WithName("created"))
        .Column(p => p.UpdatedAt, c => c.WithName("updated"))
        .PartitionKey(p => p.Id);

    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string? Name { get; set; }

    public IDictionary<string, decimal>? Prices { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
