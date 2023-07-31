namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Cloudsume.Resume.Data.Address;

internal sealed class AddressManager : UniqueDataManager<Domain, ResumeAddress>
{
    public AddressManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override Task<SampleUpdate<ResumeAddress>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeAddress>>(data, cancellationToken);
    }

    public override Task<ResumeAddress> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeAddress>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeAddress update, IReadOnlyDictionary<string, object> contents)
    {
        return new(update.Region, update.Street, DateTime.Now);
    }

    public override ResumeAddress ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
