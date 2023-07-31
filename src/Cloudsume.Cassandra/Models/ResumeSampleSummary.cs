namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleSummary : ResumeSampleData<ResumeSampleSummary, SummaryData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_summaries");
}
