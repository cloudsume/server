namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Cloudsume.Resume.Data.Address))]
public sealed class AddressData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<AddressData>("resume_address")
        .Map(a => a.Region, "region")
        .Map(a => a.Street, "street");

    public AsciiProperty? Region { get; set; }

    public TextProperty? Street { get; set; }
}
