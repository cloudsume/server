namespace Cloudsume.Template;

using System;
using System.Net;
using Cloudsume.Financial;
using NetUlid;

public sealed class CancelPurchaseFeedback
{
    public CancelPurchaseFeedback(
        Guid templateId,
        Ulid id,
        Guid userId,
        PaymentMethod paymentMethod,
        CancelPurchaseReason reason,
        IPAddress ipAddress,
        string userAgent)
    {
        this.TemplateId = templateId;
        this.Id = id;
        this.UserId = userId;
        this.PaymentMethod = paymentMethod;
        this.Reason = reason;
        this.IpAddress = ipAddress;
        this.UserAgent = userAgent;
    }

    public Guid TemplateId { get; }

    public Ulid Id { get; }

    public Guid UserId { get; }

    public PaymentMethod PaymentMethod { get; }

    public CancelPurchaseReason Reason { get; }

    public IPAddress IpAddress { get; }

    public string UserAgent { get; }
}
