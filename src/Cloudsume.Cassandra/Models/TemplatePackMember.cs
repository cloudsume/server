namespace Cloudsume.Cassandra.Models;

using System;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplatePackMember
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplatePackMember>("template_pack_members")
        .Column(m => m.PackId, c => c.WithName("pack"))
        .Column(m => m.TemplateId, c => c.WithName("template"))
        .PartitionKey(m => m.PackId)
        .ClusteringKey(m => m.TemplateId);

    public Guid PackId { get; set; }

    public Guid TemplateId { get; set; }
}
