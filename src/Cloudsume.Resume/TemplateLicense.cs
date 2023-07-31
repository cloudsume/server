namespace Cloudsume.Resume
{
    using System;
    using Cloudsume.Financial;
    using Cloudsume.Template;
    using NetUlid;

    public sealed class TemplateLicense
    {
        public TemplateLicense(Ulid id, Guid registrationId, Guid userId, PaymentMethod? payment, TemplateLicenseStatus status, DateTime updatedAt)
        {
            this.Id = id;
            this.RegistrationId = registrationId;
            this.UserId = userId;
            this.Payment = payment;
            this.Status = status;
            this.UpdatedAt = updatedAt;
        }

        public Ulid Id { get; }

        public Guid RegistrationId { get; }

        /// <summary>
        /// Gets identifier of the user who are the receiver of this license.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets payment method who created this license.
        /// </summary>
        /// <value>
        /// Payment method or <c>null</c> if this license is created by other method.
        /// </value>
        public PaymentMethod? Payment { get; }

        public TemplateLicenseStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
