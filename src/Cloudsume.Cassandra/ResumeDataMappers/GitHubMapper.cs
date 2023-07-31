namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.GitHub;

internal sealed class GitHubMapper : ResumeDataMapper<Domain, GitHubData>
{
    public override GitHubData ToCassandra(Domain domain) => new()
    {
        Username = TextProperty.From(domain.Username),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, GitHubData cassandra)
    {
        var username = cassandra.Username.ToDomain();

        return new(username, cassandra.UpdatedTime.LocalDateTime);
    }
}
