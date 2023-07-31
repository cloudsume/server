namespace Cloudsume.Cassandra.Models
{
    using System;
    using System.Collections.Generic;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class ResumeLink
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<ResumeLink>("resume_links")
            .Column(r => r.ResumeId, c => c.WithName("resume_id"))
            .Column(r => r.Id, c => c.WithName("id"))
            .Column(r => r.Name, c => c.WithName("name"))
            .Column(r => r.UserId, c => c.WithName("user_id"))
            .Column(r => r.Censorships, c => c.WithName("censorships").AsFrozen())
            .Column(r => r.CreatedAt, c => c.WithName("created_at"))
            .PartitionKey(r => r.ResumeId)
            .ClusteringKey(new[] { "id" });

        public Guid ResumeId { get; set; }

        public byte[] Id { get; set; } = Array.Empty<byte>();

        public string? Name { get; set; }

        public Guid? UserId { get; set; }

        public IEnumerable<string>? Censorships { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
