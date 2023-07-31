namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeSummary : ResumeData<ResumeSummary, SummaryData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_summaries");
    }
}
