namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleMobile : ResumeSampleData<ResumeSampleMobile, MobileData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_mobiles");
}
