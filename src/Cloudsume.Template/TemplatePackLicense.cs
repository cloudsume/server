namespace Cloudsume.Template;

using System;
using Cloudsume.Financial;
using NetUlid;

public sealed class TemplatePackLicense
{
    public TemplatePackLicense(Ulid id, Guid packId, Guid userId, PaymentMethod? payment, TemplateLicenseStatus status, DateTime updatedAt)
    {
        if (id.Time > updatedAt)
        {
            throw new ArgumentException($"The value is less than {nameof(id)}.", nameof(updatedAt));
        }

        this.Id = id;
        this.PackId = packId;
        this.UserId = userId;
        this.Payment = payment;
        this.Status = status;
        this.UpdatedAt = updatedAt;
    }

    public Ulid Id { get; }

    public Guid PackId { get; }

    public Guid UserId { get; }

    /// <summary>
    /// Gets payment method that created this license.
    /// </summary>
    /// <value>
    /// Payment method or <see langword="null"/> if this license is created by other method.
    /// </value>
    public PaymentMethod? Payment { get; }

    public TemplateLicenseStatus Status { get; set; }

    public DateTime UpdatedAt { get; set; }
}
