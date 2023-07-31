namespace Cloudsume;

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Domain = Candidate.Server.Resume.ResumeData;
using IDataAction = Cloudsume.Resume.IDataAction;

public interface IDataManager : IDataAction
{
    int MaxLocal { get; }

    int MaxGlobal { get; }

    Task<object> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default);

    Task<ISampleUpdate> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default);

    ValueTask<Domain> ToDomainAsync(object dto, IReadOnlyDictionary<string, object> contents, CancellationToken cancellationToken = default);

    ResumeData ToDto(Domain domain, IDataMappingServices services);
}
