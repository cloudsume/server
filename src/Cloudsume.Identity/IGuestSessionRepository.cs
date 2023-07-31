namespace Cloudsume.Identity;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IGuestSessionRepository
{
    Task CreateAsync(GuestSession session, CancellationToken cancellationToken = default);

    Task<GuestSession?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task SetTransferredAsync(Guid id, Guid to, DateTime time, CancellationToken cancellationToken = default);
}
