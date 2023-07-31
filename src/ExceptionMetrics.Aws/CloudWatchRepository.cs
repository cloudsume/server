namespace ExceptionMetrics.Aws;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Options;

public sealed class CloudWatchRepository : IMetricRepository
{
    private readonly CloudWatchOptions options;
    private readonly IMetricMapper mapper;
    private readonly AmazonCloudWatchClient client;

    public CloudWatchRepository(IOptions<CloudWatchOptions> options, IMetricMapper mapper)
    {
        this.options = options.Value;
        this.mapper = mapper;
        this.client = new();
    }

    public async Task WriteAsync(IEnumerable<ExceptionMetric> metrics, CancellationToken cancellationToken = default)
    {
        // Send maximum 1,000 of MetricDatum per request.
        var data = new List<MetricDatum>();

        foreach (var metric in metrics)
        {
            data.Add(this.mapper.ToCloudWatch(metric));

            if (data.Count == 1000)
            {
                await this.PutMetricDataAsync(data).ConfigureAwait(false);
                data.Clear();
            }
        }

        // Send the final chunk.
        if (data.Count != 0)
        {
            await this.PutMetricDataAsync(data).ConfigureAwait(false);
        }
    }

    private Task PutMetricDataAsync(List<MetricDatum> data, CancellationToken cancellationToken = default)
    {
        var request = new PutMetricDataRequest()
        {
            Namespace = this.options.Namespace,
            MetricData = data,
        };

        return this.client.PutMetricDataAsync(request, cancellationToken);
    }
}
