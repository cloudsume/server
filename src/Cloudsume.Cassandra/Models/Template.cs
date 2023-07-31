namespace Cloudsume.Cassandra.Models;

using System;
using System.Collections.Generic;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public sealed class Template : IRenderOptions
{
    public static readonly ITypeDefinition Mapping = ModelMapping.Create<Template>("templates")
        .Column(t => t.Id, c => c.WithName("id"))
        .Column(t => t.RegistrationId, c => c.WithName("registration_id"))
        .Column(t => t.ApplicableData, c => c.WithName("applicable_data").AsFrozen())
        .Column(t => t.ExperienceOptions, c => c.WithName("experience_options").AsFrozen())
        .Column(t => t.EducationOptions, c => c.WithName("education_options").AsFrozen())
        .Column(t => t.SkillOptions, c => c.WithName("skill_options").AsFrozen())
        .Column(t => t.Category, c => c.WithName("category"))
        .Column(t => t.ReleaseNote, c => c.WithName("release_note"))
        .PartitionKey(t => t.Id);

    public byte[] Id { get; set; } = default!;

    public Guid RegistrationId { get; set; }

    public IEnumerable<string>? ApplicableData { get; set; }

    public TemplateExperienceOptions? ExperienceOptions { get; set; }

    public TemplateEducationOptions? EducationOptions { get; set; }

    public TemplateSkillOptions? SkillOptions { get; set; }

    public sbyte Category { get; set; }

    public string? ReleaseNote { get; set; }
}
