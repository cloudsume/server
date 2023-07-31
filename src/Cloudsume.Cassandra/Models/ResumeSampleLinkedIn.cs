namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleLinkedIn : ResumeSampleData<ResumeSampleLinkedIn, LinkedInData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_linkedins");
}
