namespace ExceptionMetrics.Aws;

using System.Collections.Generic;
using Amazon.CloudWatch;

internal sealed class MetricMetadata
{
    public MetricMetadata(string name, IEnumerable<Dimension> dimensions, StandardUnit? unit, int? resolution)
    {
        this.Name = name;
        this.Dimensions = dimensions;
        this.Unit = unit;
        this.Resolution = resolution;
    }

    public string Name { get; }

    public IEnumerable<Dimension> Dimensions { get; }

    public StandardUnit? Unit { get; }

    public int? Resolution { get; }
}
