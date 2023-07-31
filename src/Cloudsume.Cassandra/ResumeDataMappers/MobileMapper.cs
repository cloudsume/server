namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Mobile;
using TelephoneNumber = Ultima.Extensions.Telephony.TelephoneNumber;

internal sealed class MobileMapper : ResumeDataMapper<Domain, MobileData>
{
    public override MobileData ToCassandra(Domain domain) => new()
    {
        Number = TelephoneProperty.From(domain.Value, v => Telephone.From(v)),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, MobileData cassandra)
    {
        var number = cassandra.Number.ToDomain(v =>
        {
            if (v is null)
            {
                return null;
            }
            else if (v.Country is null || v.Number is null)
            {
                throw new ArgumentException($"Property {nameof(cassandra.Number)} is corrupted.", nameof(cassandra));
            }

            return new TelephoneNumber(new(v.Country), v.Number);
        });

        return new(number, cassandra.UpdatedTime.LocalDateTime);
    }
}
