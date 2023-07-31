namespace Cloudsume.Identity
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        /// <summary>
        /// Gets a unique identifier of the specified user.
        /// </summary>
        /// <param name="principal">
        /// The user to get the identifier.
        /// </param>
        /// <returns>
        /// A unique identifier of the user.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="principal"/> is not authenticated or an unknown identity.
        /// </exception>
        Guid GetId(ClaimsPrincipal principal);

        Task<User?> GetAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    }
}
