namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Education;
using SubdivisionCode = Ultima.Extensions.Globalization.SubdivisionCode;

internal sealed class EducationMapper : ResumeDataMapper<Domain, EducationData>
{
    public override EducationData ToCassandra(Domain domain) => new()
    {
        Start = MonthProperty.From(domain.Start, v => Month.From(v)),
        End = MonthProperty.From(domain.End, v => Month.From(v)),
        Institute = TextProperty.From(domain.Institute),
        Region = AsciiProperty.From(domain.Region, v => v?.Value),
        Degree = TextProperty.From(domain.DegreeName),
        Grade = TextProperty.From(domain.Grade),
        Description = TextProperty.From(domain.Description),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, EducationData cassandra)
    {
        var institute = cassandra.Institute.ToDomain();
        var region = cassandra.Region.ToDomain(v => v is null ? (SubdivisionCode?)null : SubdivisionCode.Parse(v));
        var degree = cassandra.Degree.ToDomain();
        var start = cassandra.Start.ToDomain(v => v?.ToDateMonth());
        var end = cassandra.End.ToDomain(v => v?.ToDateMonth());
        var grade = cassandra.Grade.ToDomain();
        var description = cassandra.Description.ToDomain();

        return new(id, parent, institute, region, degree, start, end, grade, description, cassandra.UpdatedTime.LocalDateTime);
    }
}
