namespace Cloudsume.Cassandra;

using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cloudsume.Resume;
using Row = Cloudsume.Cassandra.Models.IResumeSampleData;

internal interface ISampleDataPayloadManager : IDataAction
{
    Task WritePayloadAsync(ResumeData data, Row row, CancellationToken cancellationToken = default);

    Task ReadPayloadAsync(ResumeData data, Row row, CancellationToken cancellationToken = default);

    Task DeletePayloadAsync(Row row, CancellationToken cancellationToken = default);
}
