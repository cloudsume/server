namespace Cloudsume.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Candidate.Server;
using Cloudsume.Server.Models;
using Domain = Candidate.Server.Resume.Data.Skill;
using SkillLevel = Candidate.Server.Resume.Data.SkillLevel;

public sealed class ResumeSkill : MultiplicativeData
{
    [JsonConstructor]
    public ResumeSkill(Guid id, Guid? @base, DataProperty<string?> name, DataProperty<SkillLevel?> level)
        : base(id, @base)
    {
        this.Name = name.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
        this.Level = level.AddValidator(new RequireDefinedAttribute());
    }

    public ResumeSkill(Domain domain)
        : base(domain)
    {
        this.Name = new(domain.Name);
        this.Level = new(domain.Level);
    }

    public DataProperty<string?> Name { get; }

    public DataProperty<SkillLevel?> Level { get; }
}
