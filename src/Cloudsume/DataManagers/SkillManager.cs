namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.Skill;

internal sealed class SkillManager : ListDataManager<Domain, ResumeSkill>
{
    public SkillManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override int MaxLocal => 10;

    public override int MaxGlobal => 100;

    public override Task<SampleUpdate<ResumeSkill>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeSkill>>(data, cancellationToken);
    }

    public override Task<ResumeSkill> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeSkill>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeSkill dto, IReadOnlyDictionary<string, object> contents)
    {
        return new(dto.Id, dto.Base, dto.Name, dto.Level, DateTime.Now);
    }

    public override ResumeSkill ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
