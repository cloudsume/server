namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeEmail : ResumeData<ResumeEmail, EmailData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_emails");
    }
}
