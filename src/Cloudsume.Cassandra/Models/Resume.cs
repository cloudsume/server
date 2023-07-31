namespace Cloudsume.Cassandra.Models
{
    using System;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public sealed class Resume
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<Resume>("resumes")
            .Column(r => r.UserId, c => c.WithName("user_id"))
            .Column(r => r.Id, c => c.WithName("id"))
            .Column(r => r.Name, c => c.WithName("name"))
            .Column(r => r.TemplateId, c => c.WithName("template_id"))
            .Column(r => r.RecruitmentConsent, c => c.WithName("recruitment_consent"))
            .Column(r => r.CreatedAt, c => c.WithName("created_at"))
            .Column(r => r.UpdatedAt, c => c.WithName("updated_at"))
            .PartitionKey(r => r.UserId)
            .ClusteringKey(r => r.Id);

        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public string? Name { get; set; }

        public byte[]? TemplateId { get; set; }

        public LocalDate? RecruitmentConsent { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
