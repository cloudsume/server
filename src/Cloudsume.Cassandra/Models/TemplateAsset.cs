namespace Cloudsume.Cassandra.Models
{
    using System;
    using global::Cassandra;

    public sealed class TemplateAsset
    {
        public static readonly UdtMap Mapping = UdtMap.For<TemplateAsset>("asset")
            .Map(a => a.Name, "name")
            .Map(a => a.Size, "size")
            .Map(a => a.LastModified, "last_modified");

        public string? Name { get; set; }

        public int Size { get; set; }

        public DateTimeOffset LastModified { get; set; }
    }
}
