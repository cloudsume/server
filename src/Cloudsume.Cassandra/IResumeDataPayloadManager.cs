namespace Cloudsume.Cassandra;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cloudsume.Resume;
using IResumeData = Cloudsume.Server.Cassandra.IResumeData;

internal interface IResumeDataPayloadManager : IDataAction
{
    Task UpdatePayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken = default);

    Task ReadPayloadAsync(ResumeData data, IResumeData row, int? index, CancellationToken cancellationToken = default);

    Task DeletePayloadAsync(Guid userId, Guid resumeId, int? index, CancellationToken cancellationToken = default);

    Task ClearPayloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default);

    Task ClearPayloadAsync(Guid userId, CultureInfo culture, CancellationToken cancellationToken = default);

    Task TransferPayloadAsync(IResumeData row, Guid to, CancellationToken cancellationToken = default);
}
