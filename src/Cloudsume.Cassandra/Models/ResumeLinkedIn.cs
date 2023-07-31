namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeLinkedIn : ResumeData<ResumeLinkedIn, LinkedInData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_linkedins");
    }
}
