namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using NetUlid;

    public interface ILinkAccessRepository
    {
        Task CreateAsync(LinkId link, IPAddress? from, CancellationToken cancellationToken = default);

        Task<LinkAccess?> GetLatestAsync(LinkId link, CancellationToken cancellationToken = default);

        Task<IEnumerable<LinkAccess>> ListAsync(LinkId link, int limit, Ulid? skipTill, CancellationToken cancellationToken = default);

        Task DeleteAsync(LinkId link, CancellationToken cancellationToken = default);
    }
}
