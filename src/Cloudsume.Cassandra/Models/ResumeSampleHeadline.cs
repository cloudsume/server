namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleHeadline : ResumeSampleData<ResumeSampleHeadline, HeadlineData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_headlines");
}
