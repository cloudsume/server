namespace Cloudsume.Financial
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IReceivingMethodRepository
    {
        Task CreateAsync(ReceivingMethod method, CancellationToken cancellationToken = default);

        Task<IEnumerable<ReceivingMethod>> ListAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<ReceivingMethod?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
