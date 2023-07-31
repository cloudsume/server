namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IResumeLinkRepository
{
    Task CreateAsync(Link link, CancellationToken cancellationToken = default);

    Task<Link?> FindAsync(LinkId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find a link with link identifier that belong to the specified resume.
    /// </summary>
    /// <param name="resumeId">
    /// Resume identifier that the link belong to.
    /// </param>
    /// <param name="linkId">
    /// Link identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A link for <paramref name="linkId"/> or <see langword="null"/> if link with <paramref name="linkId"/> does not exists or it does not belong to the
    /// specified resume.
    /// </returns>
    /// <remarks>
    /// The caller MUST ensure the resume that specified on <paramref name="resumeId"/> is belong to the user.
    /// </remarks>
    Task<Link?> GetAsync(Guid resumeId, LinkId linkId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Link>> ListAsync(Guid resumeId, CancellationToken cancellationToken = default);

    Task SetCensorshipsAsync(Guid resumeId, LinkId linkId, IReadOnlySet<LinkCensorship> censorships, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid resumeId, LinkId id, CancellationToken cancellationToken = default);

    Task TransferAsync(Guid resumeId, Guid to, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Guid resumeId, CancellationToken cancellationToken = default);
}
