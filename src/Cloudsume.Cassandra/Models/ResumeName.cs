namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeName : ResumeData<ResumeName, NameData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_names");
    }
}
