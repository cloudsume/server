namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.Summary;
using Update = Cloudsume.Server.Models.DataProperty<string?>;

internal sealed class SummarManager : UniqueDataManager<Domain, Update>
{
    public SummarManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override async Task<SampleUpdate<Update>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        var dto = await this.ReadJsonAsync<SampleUpdate<Update>>(data, cancellationToken);

        this.SetValidators(dto.Update);

        return dto;
    }

    public override async Task<Update> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        var dto = await this.ReadJsonAsync<Update>(data, cancellationToken);

        this.SetValidators(dto);

        return dto;
    }

    public override Domain ToDomain(Update update, IReadOnlyDictionary<string, object> contents)
    {
        return new(update, DateTime.Now);
    }

    public override Update ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain.Value);
    }

    private void SetValidators(Update dto)
    {
        dto.AddValidator(new StringLengthAttribute(1000) { MinimumLength = 1 });
    }
}
