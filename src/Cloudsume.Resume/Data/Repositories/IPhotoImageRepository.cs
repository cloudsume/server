namespace Candidate.Server.Resume.Data.Repositories
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPhotoImageRepository
    {
        Task UpdateAsync(Guid userId, object key, Stream image, int size, CancellationToken cancellationToken = default);

        Task<Stream?> GetAsync(Guid userId, object key, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid userId, object key, CancellationToken cancellationToken = default);
    }
}
