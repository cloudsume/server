namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeAddress : ResumeData<ResumeAddress, AddressData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_addresses");
    }
}
