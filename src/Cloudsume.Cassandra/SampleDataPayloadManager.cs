namespace Cloudsume.Cassandra;

using System;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Row = Cloudsume.Cassandra.Models.IResumeSampleData;

internal abstract class SampleDataPayloadManager<T> : ISampleDataPayloadManager where T : ResumeData
{
    public Type TargetData => typeof(T);

    public abstract Task DeletePayloadAsync(Row row, CancellationToken cancellationToken = default);

    public abstract Task ReadPayloadAsync(T data, Row row, CancellationToken cancellationToken = default);

    public abstract Task WritePayloadAsync(T data, Row row, CancellationToken cancellationToken = default);

    Task ISampleDataPayloadManager.ReadPayloadAsync(ResumeData data, Row row, CancellationToken cancellationToken)
    {
        return this.ReadPayloadAsync((T)data, row, cancellationToken);
    }

    Task ISampleDataPayloadManager.WritePayloadAsync(ResumeData data, Row row, CancellationToken cancellationToken)
    {
        return this.WritePayloadAsync((T)data, row, cancellationToken);
    }
}
