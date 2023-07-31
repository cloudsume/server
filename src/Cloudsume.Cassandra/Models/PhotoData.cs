namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Photo))]
public sealed class PhotoData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<PhotoData>("resume_photo")
        .Map(d => d.Info, "info");

    public PhotoProperty? Info { get; set; }
}
