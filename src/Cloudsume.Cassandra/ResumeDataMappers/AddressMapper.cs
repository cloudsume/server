namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Cloudsume.Resume.Data.Address;
using SubdivisionCode = Ultima.Extensions.Globalization.SubdivisionCode;

internal sealed class AddressMapper : ResumeDataMapper<Domain, AddressData>
{
    public override AddressData ToCassandra(Domain domain) => new()
    {
        Region = AsciiProperty.From(domain.Region, v => v?.Value),
        Street = TextProperty.From(domain.Street),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, AddressData cassandra)
    {
        var region = cassandra.Region.ToDomain(v => v is null ? (SubdivisionCode?)null : SubdivisionCode.Parse(v));
        var street = cassandra.Street.ToDomain();

        return new(region, street, cassandra.UpdatedTime.LocalDateTime);
    }
}
