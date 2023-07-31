namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Headline;

internal sealed class HeadlineMapper : ResumeDataMapper<Domain, HeadlineData>
{
    public override HeadlineData ToCassandra(Domain domain) => new()
    {
        Headline = TextProperty.From(domain.Value),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, HeadlineData cassandra)
    {
        var headline = cassandra.Headline.ToDomain();

        return new(headline, cassandra.UpdatedTime.LocalDateTime);
    }
}
