namespace Cloudsume.Analytics;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetUlid;

public interface IFeedbackRepository
{
    Task CreateAsync(Feedback feedback, CancellationToken cancellationToken = default);

    Task<IEnumerable<Feedback>> ListAsync(int? score, Ulid? skipTill, int limit, CancellationToken cancellationToken = default);
}
