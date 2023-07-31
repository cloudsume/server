namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Cloudsume.Resume.Data.Certificate))]
public sealed class CertificateData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<CertificateData>("resume_certificate")
        .Map(d => d.Name, "name")
        .Map(d => d.Obtained, "obtained");

    public TextProperty? Name { get; set; }

    public MonthProperty? Obtained { get; set; }
}
