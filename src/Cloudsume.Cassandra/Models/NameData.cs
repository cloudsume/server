namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Name))]
public sealed class NameData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<NameData>("resume_name")
        .Map(d => d.First, "first")
        .Map(d => d.Middle, "middle")
        .Map(d => d.Last, "last");

    public TextProperty? First { get; set; }

    public TextProperty? Middle { get; set; }

    public TextProperty? Last { get; set; }
}
