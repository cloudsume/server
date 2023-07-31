namespace ExceptionMetrics.Aws;

using System;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DimensionAttribute : Attribute
{
    public string? Name { get; set; }

    public string? Format { get; set; }

    public string? Culture { get; set; }
}
