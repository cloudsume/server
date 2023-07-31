namespace Cloudsume.Cassandra.Models
{
    using System.Net;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class ResumeLinkAccess
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<ResumeLinkAccess>("resume_link_accesses")
            .Column(r => r.LinkId, c => c.WithName("link_id"))
            .Column(r => r.Id, c => c.WithName("id"))
            .Column(r => r.From, c => c.WithName("from_ip"))
            .PartitionKey(r => r.LinkId)
            .ClusteringKey(a => a.Id, SortOrder.Descending);

        public byte[] LinkId { get; set; } = default!;

        public byte[] Id { get; set; } = default!;

        public IPAddress? From { get; set; }
    }
}
