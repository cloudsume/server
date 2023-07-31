namespace Cloudsume.Cassandra.Models
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public abstract class ResumeData<TTable, TModel> : IResumeData
        where TTable : ResumeData<TTable, TModel>
        where TModel : DataObject
    {
        public Guid UserId { get; set; }

        public Guid ResumeId { get; set; }

        public string Language { get; set; } = default!;

        public TModel? Data { get; set; }

        DataObject? IResumeData.Data
        {
            get => this.Data;
            set => this.Data = (TModel?)value;
        }

        public static async Task<IResumeData?> FetchAsync(IMapper db, Guid userId, Guid resumeId, ConsistencyLevel consistency)
        {
            var query = Cql.New("WHERE user_id = ? AND resume_id = ? AND language = ?", userId, resumeId, string.Empty).WithOptions(options =>
            {
                options.SetConsistencyLevel(consistency);
            });

            return await db.FirstOrDefaultAsync<TTable>(query);
        }

        public static async Task<IEnumerable<IResumeData>> FetchAsync(IMapper db, Cql cql)
        {
            return await db.FetchAsync<TTable>(cql);
        }

        protected static Map<TTable> CreateMapping(string table) => ModelMapping.Create<TTable>(table)
            .Column(r => r.UserId, c => c.WithName("user_id"))
            .Column(r => r.ResumeId, c => c.WithName("resume_id"))
            .Column(r => r.Language, c => c.WithName("language"))
            .Column(r => r.Data, c => c.WithName("data").AsFrozen())
            .PartitionKey(r => r.UserId)
            .ClusteringKey(r => r.ResumeId)
            .ClusteringKey(r => r.Language);
    }
}
