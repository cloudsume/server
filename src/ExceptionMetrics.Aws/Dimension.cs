namespace ExceptionMetrics.Aws;

using System;

internal sealed class Dimension
{
    public Dimension(string name, Func<ExceptionMetric, string> value)
    {
        this.Name = name;
        this.Value = value;
    }

    public string Name { get; }

    public Func<ExceptionMetric, string> Value { get; }
}
