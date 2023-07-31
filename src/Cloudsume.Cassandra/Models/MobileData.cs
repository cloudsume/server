namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Mobile))]
public sealed class MobileData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<MobileData>("resume_mobile")
        .Map(d => d.Number, "number");

    public TelephoneProperty? Number { get; set; }
}
