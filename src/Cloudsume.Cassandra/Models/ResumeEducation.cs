namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeEducation : MultiplicableResumeData<ResumeEducation, EducationData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_educations");
    }
}
