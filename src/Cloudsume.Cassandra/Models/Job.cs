namespace Cloudsume.Cassandra.Models
{
    using System;
    using System.Collections.Generic;
    using Cloudsume.Server.Cassandra;
    using global::Cassandra.Mapping;

    public sealed class Job
    {
        public static readonly ITypeDefinition Mapping = ModelMapping.Create<Job>("jobs")
            .Column(j => j.Id, c => c.WithName("id"))
            .Column(j => j.Names, c => c.WithName("names"))
            .PartitionKey(j => j.Id);

        public Guid Id { get; set; }

        public IDictionary<string, string>? Names { get; set; }
    }
}
