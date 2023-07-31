namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleAddress : ResumeSampleData<ResumeSampleAddress, AddressData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_addresses");
}
