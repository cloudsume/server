namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleEmail : ResumeSampleData<ResumeSampleEmail, EmailData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_emails");
}
