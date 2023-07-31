namespace Cloudsume.Aws
{
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    // We don't need a (distributed) mutex here due to the only cases that the data will be corrupt is when
    // there are a multiple sessions for a single user, which should never happens unless the user is trying to
    // exploit the system. Even though the user is successful to exploit this race condition it will not effect
    // the other users. So just let them faces their karma.
    internal sealed class ResumeThumbnailRepository : ThumbnailRepository<Resume>, IThumbnailRepository
    {
        public ResumeThumbnailRepository(IOptions<ResumeThumbnailRepositoryOptions> options, ILogger<S3Repository> logger)
            : base(options, logger)
        {
        }

        public async Task DeleteByResumeAsync(Resume resume, CancellationToken cancellationToken = default)
        {
            await this.DeleteAsync(resume, cancellationToken);
        }

        public Task<Thumbnail?> FindByResumeAsync(Resume resume, int page, CancellationToken cancellationToken = default)
        {
            return this.GetAsync(resume, page, cancellationToken);
        }

        protected override string GetDirectory(Resume id) => id.Id.ToString();
    }
}
