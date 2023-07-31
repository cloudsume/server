namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleName : ResumeSampleData<ResumeSampleName, NameData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_names");
}
