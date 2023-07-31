namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeExperience : MultiplicableResumeData<ResumeExperience, ExperienceData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_experiences");
    }
}
