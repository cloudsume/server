namespace ExceptionMetrics.Aws;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.CloudWatch.Model;

public sealed class MetricMapper : IMetricMapper
{
    private readonly ConcurrentDictionary<Type, MetricMetadata> meta;

    public MetricMapper()
    {
        this.meta = new();
    }

    public MetricDatum ToCloudWatch(ExceptionMetric metric)
    {
        var meta = this.meta.GetOrAdd(metric.GetType(), GetMetadata, nameof(metric));
        var data = new MetricDatum()
        {
            MetricName = meta.Name,
            Value = metric.Value,
        };

        // Get dimensions.
        var dimensions = new List<Amazon.CloudWatch.Model.Dimension>();

        foreach (var dimension in meta.Dimensions)
        {
            dimensions.Add(new()
            {
                Name = dimension.Name,
                Value = dimension.Value.Invoke(metric),
            });
        }

        if (dimensions.Count != 0)
        {
            data.Dimensions = dimensions;
        }

        // Resolution.
        if (meta.Resolution is { } resolution)
        {
            data.StorageResolution = resolution;
        }

        // Unit.
        if (meta.Unit is { } unit)
        {
            data.Unit = unit;
        }

        return data;
    }

    private static MetricMetadata GetMetadata(Type metric, string param)
    {
        // Attribute.
        var attribute = metric.GetCustomAttribute<MetricAttribute>();

        if (attribute is null)
        {
            throw new ArgumentException($"No {typeof(MetricAttribute)} is applied on {metric}.", param);
        }

        // Dimensions.
        var dimensions = new List<Dimension>();

        foreach (var property in metric.GetProperties())
        {
            // Get DimensionAttribute.
            var dimension = property.GetCustomAttribute<DimensionAttribute>();

            if (dimension is null)
            {
                continue;
            }

            // Get getter.
            var getter = property.GetGetMethod();

            if (getter is null)
            {
                throw new ArgumentException($"No public getter has been defined for {property.Name} on {metric}.", param);
            }

            dimensions.Add(new(dimension.Name ?? property.Name, BuildDimensionGetter(metric, dimension, getter)));
        }

        return new(attribute.Name, dimensions, attribute.Unit, attribute.Resolution);
    }

    private static Func<ExceptionMetric, string> BuildDimensionGetter(Type metric, DimensionAttribute attribute, MethodInfo method)
    {
        var type = method.ReturnType;
        var param = Expression.Parameter(typeof(ExceptionMetric));
        var cast = Expression.Convert(param, metric);
        var call = Expression.Call(cast, method);
        Expression<Func<ExceptionMetric, string>> lambda;

        if (type == typeof(string))
        {
            lambda = Expression.Lambda<Func<ExceptionMetric, string>>(call, param);
        }
        else if (type.IsAssignableTo(typeof(IFormattable)) && (attribute.Format is not null || attribute.Culture is not null))
        {
            var format = Expression.Constant(attribute.Format);
            var culture = Expression.Constant(attribute.Culture is null ? null : CultureInfo.GetCultureInfo(attribute.Culture));
            var @string = Expression.Call(call, nameof(IFormattable.ToString), null, format, culture);

            lambda = Expression.Lambda<Func<ExceptionMetric, string>>(@string, param);
        }
        else
        {
            var @string = Expression.Call(call, nameof(ToString), null, null);

            lambda = Expression.Lambda<Func<ExceptionMetric, string>>(@string, param);
        }

        return lambda.Compile();
    }
}
