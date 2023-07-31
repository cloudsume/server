namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Experience;
using SubdivisionCode = Ultima.Extensions.Globalization.SubdivisionCode;

internal sealed class ExperienceMapper : ResumeDataMapper<Domain, ExperienceData>
{
    public override ExperienceData ToCassandra(Domain domain) => new()
    {
        Start = MonthProperty.From(domain.Start, v => Month.From(v)),
        End = MonthProperty.From(domain.End, v => Month.From(v)),
        Title = TextProperty.From(domain.Title),
        Company = TextProperty.From(domain.Company),
        Region = AsciiProperty.From(domain.Region, v => v?.Value),
        Street = TextProperty.From(domain.Street),
        Description = TextProperty.From(domain.Description),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, ExperienceData cassandra)
    {
        var start = cassandra.Start.ToDomain(v => v?.ToDateMonth());
        var end = cassandra.End.ToDomain(v => v?.ToDateMonth());
        var title = cassandra.Title.ToDomain();
        var company = cassandra.Company.ToDomain();
        var region = cassandra.Region.ToDomain(v => v is null ? (SubdivisionCode?)null : SubdivisionCode.Parse(v));
        var street = cassandra.Street.ToDomain();
        var description = cassandra.Description.ToDomain();

        return new(id, parent, start, end, title, company, region, street, description, cassandra.UpdatedTime.LocalDateTime);
    }
}
