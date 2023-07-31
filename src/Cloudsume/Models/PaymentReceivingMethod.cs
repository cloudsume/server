namespace Cloudsume.Models
{
    using System;
    using Domain = Cloudsume.Financial.ReceivingMethod;
    using ReceivingMethodStatus = Cloudsume.Financial.ReceivingMethodStatus;

    public sealed class PaymentReceivingMethod
    {
        public PaymentReceivingMethod(Domain domain, ReceivingMethodStatus status)
        {
            this.Id = domain.Id;
            this.Status = status;
            this.CreatedAt = domain.CreatedAt;
            this.Type = domain switch
            {
                Cloudsume.Stripe.ReceivingMethod => PaymentProvider.Stripe,
                _ => throw new NotImplementedException($"No implementation for {domain.GetType()}."),
            };
        }

        public Guid Id { get; }

        public PaymentProvider Type { get; }

        public ReceivingMethodStatus Status { get; }

        public DateTime CreatedAt { get; }
    }
}
