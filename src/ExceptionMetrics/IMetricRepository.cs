namespace ExceptionMetrics;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IMetricRepository
{
    Task WriteAsync(IEnumerable<ExceptionMetric> metrics, CancellationToken cancellationToken = default);
}
