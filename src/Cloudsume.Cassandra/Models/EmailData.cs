namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.EmailAddress))]
public sealed class EmailData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<EmailData>("resume_email")
        .Map(d => d.Address, "address");

    public TextProperty? Address { get; set; }
}
