namespace Cloudsume.Financial
{
    using System;

    public abstract class ReceivingMethod
    {
        protected ReceivingMethod(Guid id, Guid userId, DateTime createdAt)
        {
            this.Id = id;
            this.UserId = userId;
            this.CreatedAt = createdAt;
        }

        public Guid Id { get; }

        /// <summary>
        /// Gets identifier of the owner account.
        /// </summary>
        /// <remarks>
        /// The value will be <see cref="Guid.Empty"/> if the owner is a system.
        /// </remarks>
        public Guid UserId { get; }

        public DateTime CreatedAt { get; }
    }
}
