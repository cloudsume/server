namespace Candidate.Server.Resume
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;

    public interface IThumbnailRepository
    {
        Task<int> UpdateAsync(
            Resume resume,
            IAsyncEnumerable<Thumbnail> thumbnails,
            CancellationToken cancellationToken = default);

        Task DeleteByResumeAsync(Resume resume, CancellationToken cancellationToken = default);

        Task<Thumbnail?> FindByResumeAsync(Resume resume, int page, CancellationToken cancellationToken = default);

        Task<int> GetPageCountAsync(Resume resume, CancellationToken cancellationToken = default);
    }
}
