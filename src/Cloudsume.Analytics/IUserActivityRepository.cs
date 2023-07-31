namespace Cloudsume.Analytics;

using System.Threading;
using System.Threading.Tasks;

public interface IUserActivityRepository
{
    Task WriteAsync(UserActivity activity, CancellationToken cancellationToken = default);
}
