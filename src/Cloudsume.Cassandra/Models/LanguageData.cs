namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Language))]
public sealed class LanguageData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<LanguageData>("resume_language")
        .Map(d => d.Language, "language")
        .Map(d => d.Comment, "comment")
        .Map(d => d.ProficiencyFlags, "proficiency_flags")
        .Map(d => d.Ilr, "ilr")
        .Map(d => d.Toefl, "toefl")
        .Map(d => d.Toeic, "toeic")
        .Map(d => d.Ielts, "ielts");

    public AsciiProperty? Language { get; set; }

    public TextProperty? Comment { get; set; }

    public sbyte ProficiencyFlags { get; set; }

    public sbyte? Ilr { get; set; }

    public sbyte? Toefl { get; set; }

    public short? Toeic { get; set; }

    public IeltsScore? Ielts { get; set; }
}
