namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Education))]
public sealed class EducationData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<EducationData>("resume_education")
        .Map(d => d.Start, "start")
        .Map(d => d.End, "end")
        .Map(d => d.Institute, "institute")
        .Map(d => d.Region, "region")
        .Map(d => d.Degree, "degree")
        .Map(d => d.Grade, "grade")
        .Map(d => d.Description, "description");

    public MonthProperty? Start { get; set; }

    public MonthProperty? End { get; set; }

    public TextProperty? Institute { get; set; }

    public AsciiProperty? Region { get; set; }

    public TextProperty? Degree { get; set; }

    public TextProperty? Grade { get; set; }

    public TextProperty? Description { get; set; }
}
