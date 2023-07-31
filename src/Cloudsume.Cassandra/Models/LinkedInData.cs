namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.LinkedIn))]
public sealed class LinkedInData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<LinkedInData>("resume_linkedin")
        .Map(d => d.Username, "username");

    public TextProperty? Username { get; set; }
}
