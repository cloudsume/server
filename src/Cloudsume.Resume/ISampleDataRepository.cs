namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume.Data;

public interface ISampleDataRepository
{
    Task WriteAsync(Guid userId, SampleData data, int? position, CancellationToken cancellationToken = default);

    Task<IEnumerable<SampleData>> GetAsync(Guid userId, Guid jobId, CultureInfo culture, string type, CancellationToken cancellationToken = default);

    Task<SampleData<Photo>?> GetPhotoAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default);

    Task<IEnumerable<SampleData>> ListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, Guid jobId, CultureInfo culture, string type, Guid? id, CancellationToken cancellationToken = default);
}
