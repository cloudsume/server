namespace Cloudsume.Cassandra.Models;

using System;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class TemplateRegistrationStats
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateRegistrationStats>("template_registration_stats")
        .Column(s => s.RegistrationId, c => c.WithName("registration_id"))
        .Column(s => s.ResumeCount, c => c.WithName("resume_count").AsCounter())
        .PartitionKey(s => s.RegistrationId);

    public Guid RegistrationId { get; set; }

    public long ResumeCount { get; set; }
}
