namespace Cloudsume
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;

    public interface IResumeHelper
    {
        Task<CompileResult> CompileAsync(
            Candidate.Server.Resume.Resume resume,
            Cloudsume.Resume.Template? template = null,
            CancellationToken cancellationToken = default);

        Task<int> UpdateThumbnailsAsync(
            Candidate.Server.Resume.Resume resume,
            Cloudsume.Resume.Template? template = null,
            CancellationToken cancellationToken = default);
    }
}
