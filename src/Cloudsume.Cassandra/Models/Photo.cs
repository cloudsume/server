namespace Cloudsume.Cassandra.Models
{
    using System;
    using global::Cassandra;
    using PhotoInfo = Candidate.Server.Resume.Data.PhotoInfo;

    public sealed class Photo
    {
        public static readonly UdtMap Mapping = UdtMap.For<Photo>("photo")
            .Map(p => p.Format, "format")
            .Map(p => p.Size, "size");

        public sbyte Format { get; set; }

        public int Size { get; set; }

        public static Photo? From(PhotoInfo? domain)
        {
            if (domain == null)
            {
                return null;
            }

            return new() { Format = Convert.ToSByte(domain.Format), Size = domain.Size };
        }
    }
}
