namespace Cloudsume.Cassandra.Models;

using System;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplateStats
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateStats>("template_stats")
        .Column(s => s.TemplateId, c => c.WithName("template_id"))
        .Column(s => s.ResumeCount, c => c.WithName("resume_count").AsCounter())
        .PartitionKey(s => s.TemplateId);

    public byte[] TemplateId { get; set; } = Array.Empty<byte>();

    public long ResumeCount { get; set; }
}
