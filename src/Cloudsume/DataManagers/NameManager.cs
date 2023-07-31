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
using Domain = Candidate.Server.Resume.Data.Name;

internal sealed class NameManager : UniqueDataManager<Domain, ResumeName>
{
    public NameManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override Task<SampleUpdate<ResumeName>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeName>>(data, cancellationToken);
    }

    public override Task<ResumeName> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeName>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeName update, IReadOnlyDictionary<string, object> contents)
    {
        return new(update.FirstName, update.MiddleName, update.LastName, DateTime.Now);
    }

    public override ResumeName ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
