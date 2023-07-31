namespace Cloudsume.Cassandra;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;

internal abstract class ResumeDataPayloadManager<T> where T : ResumeData
{
    public Type TargetData => typeof(T);

    public abstract Task ClearPayloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default);

    public abstract Task ClearPayloadAsync(Guid userId, CultureInfo culture, CancellationToken cancellationToken = default);
}
