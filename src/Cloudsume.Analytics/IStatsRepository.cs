namespace Cloudsume.Analytics;

using System.Threading;
using System.Threading.Tasks;

public interface IStatsRepository
{
    Task WriteConfigurationsLoadedAsync(CancellationToken cancellationToken = default);

    Task WriteGuestSessionCreatedAsync(CancellationToken cancellationToken = default);

    Task WriteFeedbackCreatedAsync(CancellationToken cancellationToken = default);
}
