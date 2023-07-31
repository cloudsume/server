namespace Microsoft.Extensions.DependencyInjection;

using ExceptionMetrics;

public static class IServiceCollectionExtensions
{
    public static ExceptionMetricsBuilder AddExceptionMetrics(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionMetrics, ExceptionMetrics>();

        return new(services);
    }
}
