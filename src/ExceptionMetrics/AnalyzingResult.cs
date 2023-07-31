namespace ExceptionMetrics;

using System.Collections.Generic;

public sealed class AnalyzingResult
{
    public AnalyzingResult(object? status, IEnumerable<ExceptionMetric> metrics)
    {
        this.Status = status;
        this.Metrics = metrics;
    }

    public object? Status { get; }

    public IEnumerable<ExceptionMetric> Metrics { get; }
}
