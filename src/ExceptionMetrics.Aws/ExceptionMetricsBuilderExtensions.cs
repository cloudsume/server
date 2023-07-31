namespace Microsoft.Extensions.DependencyInjection;

using System;
using ExceptionMetrics.Aws;

public static class ExceptionMetricsBuilderExtensions
{
    public static ExceptionMetricsBuilder AddCloudWatch<T>(this ExceptionMetricsBuilder builder, Action<CloudWatchOptions> options) where T : IMetricMapper
    {
        return builder.AddCloudWatch(typeof(T), options);
    }

    public static ExceptionMetricsBuilder AddCloudWatch(this ExceptionMetricsBuilder builder, Type mapper, Action<CloudWatchOptions> options)
    {
        builder.AddRepository<CloudWatchRepository>();
        builder.Services.AddSingleton(typeof(IMetricMapper), mapper);
        builder.Services.AddOptions<CloudWatchOptions>()
            .Configure(options)
            .Validate(o => o.Namespace.Length > 0, $"{nameof(CloudWatchOptions.Namespace)} is required.");

        return builder;
    }
}
