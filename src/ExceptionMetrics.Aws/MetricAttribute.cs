namespace ExceptionMetrics.Aws;

using System;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MetricAttribute : Attribute
{
    public MetricAttribute(string name)
    {
        this.Name = name;
    }

    public string Name { get; }

    public string? Unit { get; set; }

    public int? Resolution { get; set; }
}
