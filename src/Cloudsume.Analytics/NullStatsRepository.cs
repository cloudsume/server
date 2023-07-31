namespace Cloudsume.Analytics;

using System.Threading;
using System.Threading.Tasks;

internal sealed class NullStatsRepository : IStatsRepository
{
    public Task WriteConfigurationsLoadedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task WriteFeedbackCreatedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task WriteGuestSessionCreatedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
