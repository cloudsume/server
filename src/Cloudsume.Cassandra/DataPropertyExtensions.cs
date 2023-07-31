namespace Cloudsume.Cassandra;

using System;
using Cloudsume.Resume;
using Cloudsume.Resume.DataSources;

internal static class DataPropertyExtensions
{
    public static DataProperty<TValue> ToDomain<TModel, TValue>(this Models.DataProperty<TModel, TValue>? dto)
        where TModel : Models.DataProperty<TModel, TValue>, new()
    {
        return dto is null ? new(default, new FromDatabase()) : new(dto.Value, new FromDatabase(), (PropertyFlags)dto.Flags);
    }

    public static DataProperty<TOutput> ToDomain<TModel, TValue, TOutput>(this Models.DataProperty<TModel, TValue>? dto, Func<TValue?, TOutput> factory)
        where TModel : Models.DataProperty<TModel, TValue>, new()
    {
        return dto is null ? new(default, new FromDatabase()) : new(factory(dto.Value), new FromDatabase(), (PropertyFlags)dto.Flags);
    }
}
