namespace Cloudsume.Models
{
    using System.Text.Json.Serialization;
    using NetUlid;
    using Domain = Cloudsume.Resume.LinkAccess;

    public sealed class LinkAccess
    {
        public LinkAccess(Domain domain)
        {
            this.Id = domain.Id;
        }

        [JsonConstructor]
        public LinkAccess(Ulid id)
        {
            this.Id = id;
        }

        public Ulid Id { get; }
    }
}
