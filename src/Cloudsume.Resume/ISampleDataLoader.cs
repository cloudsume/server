namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;

public interface ISampleDataLoader
{
    Task<Tuple<IEnumerable<ResumeData>, IParentCollection>?> LoadAsync(
        Guid userId,
        Guid jobId,
        CultureInfo culture,
        string type,
        CancellationToken cancellationToken = default);
}
