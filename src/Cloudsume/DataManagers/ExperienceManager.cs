namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Cloudsume.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.Experience;

internal sealed class ExperienceManager : ListDataManager<Domain, ResumeExperience>
{
    public ExperienceManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override int MaxLocal => 5;

    public override int MaxGlobal => 20;

    public override Task<SampleUpdate<ResumeExperience>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeExperience>>(data, cancellationToken);
    }

    public override Task<ResumeExperience> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeExperience>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeExperience dto, IReadOnlyDictionary<string, object> contents)
    {
        return new(dto.Id, dto.Base, dto.Start, dto.End, dto.Title, dto.Company, dto.Region, dto.Street, dto.Description, DateTime.Now);
    }

    public override ResumeExperience ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
