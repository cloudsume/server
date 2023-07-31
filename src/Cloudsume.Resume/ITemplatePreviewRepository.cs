namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using NetUlid;

    public interface ITemplatePreviewRepository
    {
        Task<int> CreateAsync(Ulid templateId, IAsyncEnumerable<Thumbnail> previews, CancellationToken cancellationToken = default);

        Task<int> GetPageCountAsync(Ulid templateId, CancellationToken cancellationToken = default);

        Task<Thumbnail?> GetAsync(Ulid templateId, int page, CancellationToken cancellationToken = default);
    }
}
