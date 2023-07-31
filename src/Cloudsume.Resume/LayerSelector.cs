namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Linq;
using Candidate.Server.Resume;
using Cloudsume.Resume.DataSources;

internal ref struct LayerSelector<T> where T : ResumeData
{
    private readonly IEnumerable<T> layers;
    private DateTime? lastUpdated;

    public LayerSelector(IEnumerable<T> layers)
    {
        this.layers = layers;
        this.lastUpdated = null;
    }

    public DateTime LastUpdated => this.lastUpdated ?? this.layers.Max(d => d.UpdatedAt);

    public DataProperty<TProperty> For<TProperty>(Func<T, DataProperty<TProperty>> selector)
    {
        foreach (var layer in this.layers)
        {
            var property = this.GetProperty(layer, selector);

            if (property is not null)
            {
                return property.WithSource(new FromAggregator());
            }
        }

        return new(default, new FromAggregator());
    }

    public T? Select<TProperty>(Func<T, DataProperty<TProperty>> by)
    {
        foreach (var layer in this.layers)
        {
            if (this.GetProperty(layer, by) is not null)
            {
                return layer;
            }
        }

        return null;
    }

    private DataProperty<TProperty>? GetProperty<TProperty>(T data, Func<T, DataProperty<TProperty>> selector)
    {
        var property = selector(data);

        if (property.IsFallback)
        {
            return null;
        }

        if (this.lastUpdated is null || data.UpdatedAt > this.lastUpdated)
        {
            this.lastUpdated = data.UpdatedAt;
        }

        return property;
    }
}
