namespace Cloudsume.Cassandra.Models;

using System;
using Cloudsume.Server.Cassandra;
using global::Cassandra;
using global::Cassandra.Mapping;

public sealed class RecruitmentRevoke
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<RecruitmentRevoke>("recruitment_revokes")
        .Column(r => r.Date, c => c.WithName("date"))
        .Column(r => r.UserId, c => c.WithName("user"))
        .Column(r => r.ResumeId, c => c.WithName("resume"))
        .PartitionKey(r => r.Date)
        .ClusteringKey(r => r.UserId)
        .ClusteringKey(r => r.ResumeId);

    public LocalDate Date { get; set; } = new(1, 1, 1);

    public Guid UserId { get; set; }

    public Guid ResumeId { get; set; }
}
