namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.LinkedIn;

internal sealed class LinkedInMapper : ResumeDataMapper<Domain, LinkedInData>
{
    public override LinkedInData ToCassandra(Domain domain) => new()
    {
        Username = TextProperty.From(domain.Username),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, LinkedInData cassandra)
    {
        var username = cassandra.Username.ToDomain();

        return new(username, cassandra.UpdatedTime.LocalDateTime);
    }
}
