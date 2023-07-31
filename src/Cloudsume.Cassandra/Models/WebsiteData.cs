namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Cloudsume.Resume.Data.Website))]
public sealed class WebsiteData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<WebsiteData>("resume_website")
        .Map(d => d.Url, "url");

    public TextProperty? Url { get; set; }
}
