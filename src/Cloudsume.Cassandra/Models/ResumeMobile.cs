namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeMobile : ResumeData<ResumeMobile, MobileData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_mobiles");
    }
}
