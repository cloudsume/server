namespace Cloudsume.Cassandra.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudsume.Server.Cassandra;
using global::Cassandra.Mapping;

public abstract class ResumeSampleData<TTable, TData> : IResumeSampleData
    where TTable : ResumeSampleData<TTable, TData>
    where TData : DataObject
{
    public Guid Owner { get; set; }

    public Guid TargetJob { get; set; }

    public string Culture { get; set; } = string.Empty;

    public TData? Data { get; set; }

    public Guid? ParentJob { get; set; }

    DataObject? IResumeSampleData.Data
    {
        get => this.Data;
        set => this.Data = (TData?)value;
    }

    public static async Task<IEnumerable<IResumeSampleData>> FetchAsync(IMapper db, Cql cql)
    {
        return await db.FetchAsync<TTable>(cql);
    }

    protected static Map<TTable> CreateMapping(string table) => ModelMapping.Create<TTable>(table)
        .Column(d => d.Owner, c => c.WithName("owner"))
        .Column(d => d.TargetJob, c => c.WithName("job"))
        .Column(d => d.Culture, c => c.WithName("culture"))
        .Column(d => d.Data, c => c.WithName("data").AsFrozen())
        .Column(d => d.ParentJob, c => c.WithName("parent_job"))
        .PartitionKey(d => d.Owner)
        .ClusteringKey(d => d.TargetJob)
        .ClusteringKey(d => d.Culture);
}
