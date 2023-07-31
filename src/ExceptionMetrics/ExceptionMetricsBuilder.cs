namespace Microsoft.Extensions.DependencyInjection;

using System;
using ExceptionMetrics;

public sealed class ExceptionMetricsBuilder
{
    public ExceptionMetricsBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    public IServiceCollection Services { get; }

    public ExceptionMetricsBuilder AddAnalyzer<T>() where T : IExceptionAnalyzer => this.AddAnalyzer(typeof(T));

    public ExceptionMetricsBuilder AddAnalyzer(Type analyzer)
    {
        this.Services.AddSingleton(typeof(IExceptionAnalyzer), analyzer);
        return this;
    }

    public ExceptionMetricsBuilder AddRepository<T>() where T : IMetricRepository => this.AddRepository(typeof(T));

    public ExceptionMetricsBuilder AddRepository(Type repository)
    {
        this.Services.AddSingleton(typeof(IMetricRepository), repository);
        return this;
    }
}
