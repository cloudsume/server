namespace Cloudsume.Cassandra;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cloudsume.Server.Cassandra;

internal abstract class UniqueResumeDataPayloadManager<T> : ResumeDataPayloadManager<T>, IResumeDataPayloadManager where T : ResumeData
{
    public abstract Task UpdatePayloadAsync(T data, IResumeData row, CancellationToken cancellationToken = default);

    public abstract Task ReadPayloadAsync(T data, IResumeData row, CancellationToken cancellationToken = default);

    public abstract Task DeletePayloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default);

    public abstract Task DeletePayloadAsync(Guid userId, CultureInfo culture, CancellationToken cancellationToken = default);

    public override sealed Task ClearPayloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default)
    {
        return this.DeletePayloadAsync(userId, resumeId, cancellationToken);
    }

    public override sealed Task ClearPayloadAsync(Guid userId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        return this.DeletePayloadAsync(userId, culture, cancellationToken);
    }

    public abstract Task TransferPayloadAsync(IResumeData row, Guid to, CancellationToken cancellationToken);

    Task IResumeDataPayloadManager.DeletePayloadAsync(Guid userId, Guid resumeId, int? index, CancellationToken cancellationToken)
    {
        if (index != null)
        {
            throw new ArgumentException("The value must be null.", nameof(index));
        }

        return this.DeletePayloadAsync(userId, resumeId, cancellationToken);
    }

    Task IResumeDataPayloadManager.ReadPayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken)
    {
        if (index != null)
        {
            throw new ArgumentException("The value must be null.", nameof(index));
        }

        return this.ReadPayloadAsync((T)data, row, cancellationToken);
    }

    Task IResumeDataPayloadManager.UpdatePayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken)
    {
        if (index != null)
        {
            throw new ArgumentException("The value must be null.", nameof(index));
        }

        return this.UpdatePayloadAsync((T)data, row, cancellationToken);
    }
}
