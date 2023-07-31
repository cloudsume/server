namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Summary;

internal sealed class SummaryMapper : ResumeDataMapper<Domain, SummaryData>
{
    public override SummaryData ToCassandra(Domain domain) => new()
    {
        Summary = TextProperty.From(domain.Value),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, SummaryData cassandra)
    {
        var value = cassandra.Summary.ToDomain();

        return new(value, cassandra.UpdatedTime.LocalDateTime);
    }
}
