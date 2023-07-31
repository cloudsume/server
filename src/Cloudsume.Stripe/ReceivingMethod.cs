namespace Cloudsume.Stripe
{
    using System;

    public sealed class ReceivingMethod : Cloudsume.Financial.ReceivingMethod
    {
        public ReceivingMethod(Guid id, Guid userId, string accountId, DateTime createdAt)
            : base(id, userId, createdAt)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        /// Gets identifier of the Stripe account.
        /// </summary>
        public string AccountId { get; }
    }
}
