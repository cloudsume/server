namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeSkill : MultiplicableResumeData<ResumeSkill, SkillData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_skills");
    }
}
