namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleSkill : MultiplicableSampleData<ResumeSampleSkill, SkillData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_skills");
}
