namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;

    public interface IWorkspacePreviewRepository
    {
        Task<int> UpdateAsync(Guid registrationId, IAsyncEnumerable<Thumbnail> previews, CancellationToken cancellationToken = default);

        Task<Thumbnail?> GetAsync(Guid registrationId, int page, CancellationToken cancellationToken = default);

        Task<int> GetPageCountAsync(Guid registrationId, CancellationToken cancellationToken = default);
    }
}
