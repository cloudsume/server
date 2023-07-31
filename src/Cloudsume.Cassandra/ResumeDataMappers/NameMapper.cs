namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Name;

internal sealed class NameMapper : ResumeDataMapper<Domain, NameData>
{
    public override NameData ToCassandra(Domain domain) => new()
    {
        First = TextProperty.From(domain.FirstName),
        Middle = TextProperty.From(domain.MiddleName),
        Last = TextProperty.From(domain.LastName),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, NameData cassandra)
    {
        var first = cassandra.First.ToDomain();
        var middle = cassandra.Middle.ToDomain();
        var last = cassandra.Last.ToDomain();

        return new(first, middle, last, cassandra.UpdatedTime.LocalDateTime);
    }
}
