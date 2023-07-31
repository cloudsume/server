namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.Education;

internal sealed class EducationManager : ListDataManager<Domain, ResumeEducation>
{
    public EducationManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override int MaxLocal => 3;

    public override int MaxGlobal => 20;

    public override Task<SampleUpdate<ResumeEducation>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeEducation>>(data, cancellationToken);
    }

    public override Task<ResumeEducation> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeEducation>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeEducation dto, IReadOnlyDictionary<string, object> contents)
    {
        return new Domain(
            dto.Id,
            dto.Base,
            dto.Institute,
            dto.Region,
            dto.DegreeName,
            dto.Start,
            dto.End,
            dto.Grade,
            dto.Description,
            DateTime.Now);
    }

    public override ResumeEducation ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
