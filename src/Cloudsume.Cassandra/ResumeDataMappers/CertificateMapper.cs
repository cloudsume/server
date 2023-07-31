namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Cloudsume.Resume.Data.Certificate;

internal sealed class CertificateMapper : ResumeDataMapper<Domain, CertificateData>
{
    public override CertificateData ToCassandra(Domain domain) => new()
    {
        Name = TextProperty.From(domain.Name),
        Obtained = MonthProperty.From(domain.Obtained, v => Month.From(v)),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, CertificateData cassandra)
    {
        var name = cassandra.Name.ToDomain();
        var obtained = cassandra.Obtained.ToDomain(v => v?.ToDateMonth());

        return new(id, parent, name, obtained, cassandra.UpdatedTime.LocalDateTime);
    }
}
