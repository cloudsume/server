namespace Cloudsume.Cassandra;

using System;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cloudsume.Server.Cassandra;

internal abstract class MultiplicableResumeDataPayloadManager<T> : ResumeDataPayloadManager<T>, IResumeDataPayloadManager where T : ResumeData
{
    public abstract Task UpdatePayloadAsync(T data, IMultiplicativeResumeData row, int? index, CancellationToken cancellationToken = default);

    public abstract Task ReadPayloadAsync(T data, IMultiplicativeResumeData row, int? index, CancellationToken cancellationToken = default);

    public abstract Task DeletePayloadAsync(Guid userId, Guid resumeId, int index, CancellationToken cancellationToken = default);

    public abstract Task TransferPayloadAsync(IMultiplicativeResumeData row, Guid to, CancellationToken cancellationToken);

    Task IResumeDataPayloadManager.DeletePayloadAsync(Guid userId, Guid resumeId, int? index, CancellationToken cancellationToken)
    {
        if (index == null)
        {
            throw new ArgumentNullException(nameof(index));
        }

        return this.DeletePayloadAsync(userId, resumeId, index.Value, cancellationToken);
    }

    Task IResumeDataPayloadManager.ReadPayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken)
    {
        return this.ReadPayloadAsync((T)data, (IMultiplicativeResumeData)row, index, cancellationToken);
    }

    Task IResumeDataPayloadManager.TransferPayloadAsync(IResumeData row, Guid to, CancellationToken cancellationToken)
    {
        return this.TransferPayloadAsync((IMultiplicativeResumeData)row, to, cancellationToken);
    }

    Task IResumeDataPayloadManager.UpdatePayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken)
    {
        return this.UpdatePayloadAsync((T)data, (IMultiplicativeResumeData)row, index, cancellationToken);
    }
}
