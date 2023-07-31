namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Summary))]
public sealed class SummaryData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<SummaryData>("resume_summary")
        .Map(d => d.Summary, "summary");

    public TextProperty? Summary { get; set; }
}
