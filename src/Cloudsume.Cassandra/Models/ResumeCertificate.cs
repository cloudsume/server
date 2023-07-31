namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeCertificate : MultiplicableResumeData<ResumeCertificate, CertificateData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_certificates");
}
