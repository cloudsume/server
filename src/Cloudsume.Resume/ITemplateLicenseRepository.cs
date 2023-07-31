namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Financial;
    using Cloudsume.Template;
    using NetUlid;

    public interface ITemplateLicenseRepository
    {
        Task CreateAsync(TemplateLicense license, CancellationToken cancellationToken = default);

        Task<TemplateLicense?> GetAsync(Guid userId, Guid registrationId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TemplateLicense>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TemplateLicense>> ListByRegistrationAsync(Guid registrationId, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid registrationId, Ulid id, CancellationToken cancellationToken = default);

        Task SetPaymentAsync(Guid registrationId, Ulid id, PaymentMethod? payment, CancellationToken cancellationToken = default);

        Task SetStatusAsync(Guid registrationId, Ulid id, TemplateLicenseStatus status, CancellationToken cancellationToken = default);

        Task SetUpdatedAsync(Guid registrationId, Ulid id, DateTime updatedAt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfer all licenses with status equal to <see cref="TemplateLicenseStatus.Valid"/> from one user to another user.
        /// </summary>
        /// <param name="from">
        /// A user identifier to transfer from.
        /// </param>
        /// <param name="to">
        /// A user identifier to transfer to.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous transfer operation.
        /// </returns>
        Task TransferAsync(Guid from, Guid to, CancellationToken cancellationToken = default);
    }
}
