namespace Cloudsume.Cassandra.Models;

using System;
using global::Cassandra.Mapping;

public abstract class MultiplicableSampleData<TTable, TModel> : ResumeSampleData<TTable, TModel>, IMultiplicableSampleData
    where TTable : MultiplicableSampleData<TTable, TModel>
    where TModel : DataObject
{
    public Guid Id { get; set; }

    public sbyte Position { get; set; }

    public Guid? Parent { get; set; }

    protected static new Map<TTable> CreateMapping(string table) => ResumeSampleData<TTable, TModel>.CreateMapping(table)
        .Column(d => d.Id, c => c.WithName("id"))
        .Column(d => d.Position, c => c.WithName("position"))
        .Column(d => d.Parent, c => c.WithName("parent"))
        .ClusteringKey(d => d.Id);
}
