namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Experience))]
public sealed class ExperienceData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<ExperienceData>("resume_experience")
        .Map(d => d.Start, "start")
        .Map(d => d.End, "end")
        .Map(d => d.Title, "title")
        .Map(d => d.Company, "company")
        .Map(d => d.Region, "region")
        .Map(d => d.Street, "street")
        .Map(d => d.Description, "description");

    public MonthProperty? Start { get; set; }

    public MonthProperty? End { get; set; }

    public TextProperty? Title { get; set; }

    public TextProperty? Company { get; set; }

    public AsciiProperty? Region { get; set; }

    public TextProperty? Street { get; set; }

    public TextProperty? Description { get; set; }
}
