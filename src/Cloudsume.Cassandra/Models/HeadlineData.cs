namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Headline))]
public sealed class HeadlineData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<HeadlineData>("resume_headline")
        .Map(d => d.Headline, "headline");

    public TextProperty? Headline { get; set; }
}
