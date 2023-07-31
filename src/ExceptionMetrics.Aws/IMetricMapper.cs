namespace ExceptionMetrics.Aws;

using Amazon.CloudWatch.Model;

public interface IMetricMapper
{
    MetricDatum ToCloudWatch(ExceptionMetric metric);
}
