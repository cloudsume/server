namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Skill;
using SkillLevel = Candidate.Server.Resume.Data.SkillLevel;

internal sealed class SkillMapper : ResumeDataMapper<Domain, SkillData>
{
    public override SkillData ToCassandra(Domain domain) => new()
    {
        Skill = TextProperty.From(domain.Name),
        Level = SByteProperty.From(domain.Level, v => v is { } l ? Convert.ToSByte(l) : null),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, SkillData cassandra)
    {
        var name = cassandra.Skill.ToDomain();
        var level = cassandra.Level.ToDomain(v => (SkillLevel?)v);

        return new(id, parent, name, level, cassandra.UpdatedTime.LocalDateTime);
    }
}
