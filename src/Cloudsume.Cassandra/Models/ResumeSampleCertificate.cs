namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleCertificate : MultiplicableSampleData<ResumeSampleCertificate, CertificateData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_certificates");
}
