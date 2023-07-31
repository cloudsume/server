namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSamplePhoto : ResumeSampleData<ResumeSamplePhoto, PhotoData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_photos");
}
