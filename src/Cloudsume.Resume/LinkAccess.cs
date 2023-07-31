namespace Cloudsume.Resume
{
    using System.Net;
    using NetUlid;

    public sealed class LinkAccess
    {
        public LinkAccess(Ulid id, IPAddress? from)
        {
            this.Id = id;
            this.From = from;
        }

        public Ulid Id { get; }

        public IPAddress? From { get; }
    }
}
