namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.EmailAddress;
using Update = Cloudsume.Server.Models.DataProperty<System.Net.Mail.MailAddress?>;

internal sealed class EmailAddressManager : UniqueDataManager<Domain, Update>
{
    public EmailAddressManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override Task<SampleUpdate<Update>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<Update>>(data, cancellationToken);
    }

    public override Task<Update> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<Update>(data, cancellationToken);
    }

    public override Domain ToDomain(Update update, IReadOnlyDictionary<string, object> contents)
    {
        return new(update, DateTime.Now);
    }

    public override Update ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain.Value);
    }
}
