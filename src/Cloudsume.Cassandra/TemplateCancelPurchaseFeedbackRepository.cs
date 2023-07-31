namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Financial;
using Cloudsume.Template;
using Cornot;
using NetUlid;

internal sealed class TemplateCancelPurchaseFeedbackRepository : ICancelPurchaseFeedbackRepository
{
    private readonly IMapperFactory db;

    public TemplateCancelPurchaseFeedbackRepository(IMapperFactory db)
    {
        this.db = db;
    }

    public Task CreateAsync(CancelPurchaseFeedback feedback, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var row = new Models.TemplateCancelPurchaseFeedback()
        {
            TemplateId = feedback.TemplateId,
            Id = feedback.Id.ToByteArray(),
            UserId = feedback.UserId,
            Reason = Convert.ToSByte(feedback.Reason),
            Ip = feedback.IpAddress,
            UserAgent = string.IsNullOrEmpty(feedback.UserAgent) ? null : feedback.UserAgent,
        };

        switch (feedback.PaymentMethod)
        {
            case Cloudsume.Stripe.PaymentMethod m:
                row.StripePaymentIntent = m.Id;
                break;
            default:
                throw new ArgumentException($"Unknown payment method {feedback.PaymentMethod.GetType()}.", nameof(feedback));
        }

        return db.InsertAsync(row);
    }

    public async Task<IEnumerable<CancelPurchaseFeedback>> ListAsync(
        Guid templateId,
        Ulid? skipTill = null,
        int max = 100,
        CancellationToken cancellationToken = default)
    {
        if (max <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        var db = this.db.CreateMapper();

        // Build a query.
        var query = new StringBuilder("WHERE template = ?");
        var args = new List<object>() { templateId };

        if (skipTill is { } skip)
        {
            query.Append(" AND id < ?");
            args.Add(skip.ToByteArray());
        }

        query.Append(" ORDER BY id DESC LIMIT ?");
        args.Add(max);

        // Execute.
        var rows = await db.FetchAsync<Models.TemplateCancelPurchaseFeedback>(query.ToString(), args.ToArray()).ConfigureAwait(false);

        return rows.Select(this.ToDomain).ToArray();
    }

    private CancelPurchaseFeedback ToDomain(Models.TemplateCancelPurchaseFeedback row)
    {
        PaymentMethod payment;

        if (row.StripePaymentIntent is { } stripePaymentIntent)
        {
            payment = new Cloudsume.Stripe.PaymentMethod(stripePaymentIntent);
        }
        else
        {
            throw new DataCorruptionException(row, "No payment method.");
        }

        if (row.Ip is not { } ip)
        {
            throw new DataCorruptionException(row, $"{nameof(row.Ip)} is null.");
        }

        return new(row.TemplateId, new(row.Id), row.UserId, payment, (CancelPurchaseReason)row.Reason, ip, row.UserAgent ?? string.Empty);
    }
}
