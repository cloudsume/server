namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.Skill))]
public sealed class SkillData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<SkillData>("resume_skill")
        .Map(d => d.Skill, "skill")
        .Map(d => d.Level, "level");

    public TextProperty? Skill { get; set; }

    public SByteProperty? Level { get; set; }
}
