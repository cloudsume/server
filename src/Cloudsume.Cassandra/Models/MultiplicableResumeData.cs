namespace Cloudsume.Cassandra.Models
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public abstract class MultiplicableResumeData<TTable, TModel> : ResumeData<TTable, TModel>, IMultiplicativeResumeData
        where TTable : MultiplicableResumeData<TTable, TModel>
        where TModel : DataObject
    {
        public sbyte Index { get; set; }

        public Guid Id { get; set; }

        public Guid? BaseId { get; set; }

        public static new async Task<IEnumerable<IMultiplicativeResumeData>> FetchAsync(IMapper db, Guid userId, Guid resumeId, ConsistencyLevel consistency)
        {
            var query = Cql.New(
                "WHERE user_id = ? AND resume_id = ? AND language = ? ORDER BY resume_id ASC, language ASC, position ASC",
                userId,
                resumeId,
                string.Empty);

            query.WithOptions(options => options.SetConsistencyLevel(consistency));

            return await db.FetchAsync<TTable>(query);
        }

        protected static new Map<TTable> CreateMapping(string table) => ResumeData<TTable, TModel>.CreateMapping(table)
            .Column(r => r.Index, c => c.WithName("position"))
            .Column(r => r.Id, c => c.WithName("id"))
            .Column(r => r.BaseId, c => c.WithName("base_id"))
            .ClusteringKey(r => r.Index)
            .ClusteringKey(r => r.Id);
    }
}
