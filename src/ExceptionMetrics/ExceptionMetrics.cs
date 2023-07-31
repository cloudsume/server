namespace ExceptionMetrics;

using System;
using System.Threading.Tasks;

public sealed class ExceptionMetrics : IExceptionMetrics
{
    private readonly IExceptionAnalyzer analyzer;
    private readonly IMetricRepository repository;

    public ExceptionMetrics(IExceptionAnalyzer analyzer, IMetricRepository repository)
    {
        this.analyzer = analyzer;
        this.repository = repository;
    }

    public async Task<object?> WriteAsync(Exception exception)
    {
        var result = this.analyzer.Analyze(exception);

        await this.repository.WriteAsync(result.Metrics).ConfigureAwait(false);

        return result.Status;
    }
}
