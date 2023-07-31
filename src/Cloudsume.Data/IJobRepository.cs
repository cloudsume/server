namespace Cloudsume.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IJobRepository
    {
        Task CreateAsync(Job job, CancellationToken cancellationToken = default);

        Task<Job?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Job>> ListAsync(CancellationToken cancellationToken = default);

        Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
