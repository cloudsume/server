namespace Cloudsume.Aws;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Cloudsume.Analytics;
using Microsoft.Extensions.Options;

internal sealed class StatsRepository : IDisposable, IStatsRepository
{
    private readonly StatsRepositoryOptions options;
    private readonly AmazonCloudWatchClient cloudWatch;

    public StatsRepository(IOptions<StatsRepositoryOptions> options)
    {
        this.options = options.Value;
        this.cloudWatch = new();
    }

    public void Dispose()
    {
        this.cloudWatch.Dispose();
    }

    public Task WriteConfigurationsLoadedAsync(CancellationToken cancellationToken = default)
    {
        var metrics = new List<MetricDatum>()
        {
            new MetricDatum()
            {
                MetricName = "ConfigurationsLoaded",
                Unit = StandardUnit.Count,
                Value = 1,
            },
        };

        return this.PutMetricDataAsync(metrics, cancellationToken);
    }

    public Task WriteFeedbackCreatedAsync(CancellationToken cancellationToken = default)
    {
        var metrics = new List<MetricDatum>()
        {
            new MetricDatum()
            {
                MetricName = "FeedbackCreated",
                Unit = StandardUnit.Count,
                Value = 1,
            },
        };

        return this.PutMetricDataAsync(metrics, cancellationToken);
    }

    public Task WriteGuestSessionCreatedAsync(CancellationToken cancellationToken = default)
    {
        var metrics = new List<MetricDatum>()
        {
            new MetricDatum()
            {
                MetricName = "GuestSessionCreated",
                Unit = StandardUnit.Count,
                Value = 1,
            },
        };

        return this.PutMetricDataAsync(metrics, cancellationToken);
    }

    private Task PutMetricDataAsync(List<MetricDatum> metrics, CancellationToken cancellationToken = default)
    {
        var request = new PutMetricDataRequest()
        {
            Namespace = this.options.Namespace,
            MetricData = metrics,
        };

        return this.cloudWatch.PutMetricDataAsync(request, cancellationToken);
    }
}
