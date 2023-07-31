namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Cloudsume.Resume.Data.Certificate;

internal sealed class CertificateManager : ListDataManager<Domain, ResumeCertificate>
{
    public CertificateManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override int MaxLocal => 3;

    public override int MaxGlobal => 20;

    public override Task<SampleUpdate<ResumeCertificate>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeCertificate>>(data, cancellationToken);
    }

    public override Task<ResumeCertificate> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeCertificate>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeCertificate dto, IReadOnlyDictionary<string, object> contents)
    {
        return new(dto.Id, dto.Base, dto.Name, dto.Obtained, DateTime.Now);
    }

    public override ResumeCertificate ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
