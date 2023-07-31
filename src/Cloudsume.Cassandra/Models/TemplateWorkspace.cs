namespace Cloudsume.Cassandra.Models
{
    using System;
    using System.Collections.Generic;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class TemplateWorkspace : IRenderOptions
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<TemplateWorkspace>("workspaces")
            .Column(w => w.RegistrationId, c => c.WithName("registration_id"))
            .Column(w => w.PreviewJob, c => c.WithName("preview_job"))
            .Column(w => w.ApplicableData, c => c.WithName("applicable_data").AsFrozen())
            .Column(w => w.ExperienceOptions, c => c.WithName("experience_options").AsFrozen())
            .Column(w => w.EducationOptions, c => c.WithName("education_options").AsFrozen())
            .Column(w => w.SkillOptions, c => c.WithName("skill_options").AsFrozen())
            .Column(w => w.Assets, c => c.WithName("assets").WithFrozenValue())
            .PartitionKey(w => w.RegistrationId);

        public Guid RegistrationId { get; set; }

        public Guid? PreviewJob { get; set; }

        public IEnumerable<string>? ApplicableData { get; set; }

        public TemplateExperienceOptions? ExperienceOptions { get; set; }

        public TemplateEducationOptions? EducationOptions { get; set; }

        public TemplateSkillOptions? SkillOptions { get; set; }

        public IDictionary<string, TemplateAsset>? Assets { get; set; }
    }
}
