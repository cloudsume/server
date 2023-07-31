namespace Cloudsume;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.ResumeData;

internal abstract class ListDataManager<TDomain, TUpdate> : DataManager, IDataManager
    where TDomain : Candidate.Server.Resume.MultiplicativeData
    where TUpdate : notnull
{
    protected ListDataManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public Type TargetData => typeof(TDomain);

    public abstract int MaxLocal { get; }

    public abstract int MaxGlobal { get; }

    public abstract Task<TUpdate> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default);

    public abstract Task<SampleUpdate<TUpdate>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default);

    public virtual TDomain ToDomain(TUpdate dto, IReadOnlyDictionary<string, object> contents)
    {
        throw new NotImplementedException($"The derived class need to override either '{nameof(this.ToDomain)}' or '{nameof(this.ToDomainAsync)}'.");
    }

    public virtual ValueTask<TDomain> ToDomainAsync(TUpdate dto, IReadOnlyDictionary<string, object> contents, CancellationToken cancellationToken = default)
    {
        return new(this.ToDomain(dto, contents));
    }

    public abstract TUpdate ToDto(TDomain domain, IDataMappingServices services);

    async Task<ISampleUpdate> IDataManager.ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken)
    {
        return await this.ReadSampleUpdateAsync(data, cancellationToken);
    }

    async Task<object> IDataManager.ReadUpdateAsync(Stream data, CancellationToken cancellationToken)
    {
        return await this.ReadUpdateAsync(data, cancellationToken);
    }

    async ValueTask<Domain> IDataManager.ToDomainAsync(object dto, IReadOnlyDictionary<string, object> contents, CancellationToken cancellationToken)
    {
        return await this.ToDomainAsync((TUpdate)dto, contents, cancellationToken);
    }

    ResumeData IDataManager.ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain.Type, domain.UpdatedAt, this.ToDto((TDomain)domain, services));
    }
}
