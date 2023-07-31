namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeHeadline : ResumeData<ResumeHeadline, HeadlineData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_headlines");
    }
}
