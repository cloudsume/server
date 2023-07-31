namespace Cloudsume.Stripe;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Identity;
using global::Stripe;
using Ultima.Extensions.Currency;
using PaymentCancelReason = Cloudsume.Financial.PaymentCancelReason;
using PaymentInfo = Cloudsume.Financial.PaymentInfo;
using PaymentMethodStatus = Cloudsume.Financial.PaymentMethodStatus;
using ReceivingMethodStatus = Cloudsume.Financial.ReceivingMethodStatus;

internal sealed class ReceivingMethodServiceAdaptor : Cloudsume.Financial.ReceivingMethodServiceAdaptor<ReceivingMethod, PaymentMethod>
{
    private readonly IStripeClient client;
    private readonly IUserRepository users;

    public ReceivingMethodServiceAdaptor(IStripeClient client, IUserRepository users)
    {
        this.client = client;
        this.users = users;
    }

    public override Task CancelPaymentAsync(PaymentMethod method, PaymentCancelReason reason, CancellationToken cancellationToken)
    {
        // Setup the request.
        var request = new PaymentIntentCancelOptions();

        switch (reason)
        {
            case PaymentCancelReason.PayerRequested:
                request.CancellationReason = "requested_by_customer";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(reason));
        }

        // Send the request.
        var service = new PaymentIntentService(this.client);

        return service.CancelAsync(method.Id, request, null, cancellationToken);
    }

    public override async Task<Cloudsume.Financial.PaymentMethod?> CreatePaymentMethodAsync(
        ReceivingMethod method,
        PaymentInfo info,
        decimal fee,
        CancellationToken cancellationToken = default)
    {
        // Check if account can accept payment.
        var account = await this.GetAccountAsync(method.AccountId, cancellationToken);

        if (!account.ChargesEnabled)
        {
            return null;
        }

        // Create payment intent.
        var payer = await this.users.GetAsync(info.Payer, cancellationToken);
        var currency = CurrencyInfo.Get(info.Currency);
        var service = new PaymentIntentService(this.client);
        var request = new PaymentIntentCreateOptions()
        {
            Currency = info.Currency.Value,
            Amount = currency.GetMinorUnitAmount(info.Amount),
            Description = info.Description,
            Metadata = new()
            {
                { "payer", (payer?.Id ?? this.users.GetId(info.Payer)).ToString() },
            },
            AutomaticPaymentMethods = new()
            {
                Enabled = true,
            },
        };

        if (method.UserId != Guid.Empty)
        {
            var transfer = info.Amount - fee;

            if (transfer > 0m)
            {
                request.TransferData = new()
                {
                    Amount = currency.GetMinorUnitAmount(transfer),
                    Destination = method.AccountId,
                };
            }
        }

        if (payer != null && payer.EmailVerified)
        {
            request.ReceiptEmail = payer.Email.Address;
        }

        var intent = await service.CreateAsync(request, null, cancellationToken);

        return new FullPaymentMethod(intent);
    }

    public override async Task<Uri?> GetSetupUriAsync(ReceivingMethod method, Uri returnUri, CancellationToken cancellationToken)
    {
        // Check if account onboarding completed.
        var account = await this.GetAccountAsync(method.AccountId, cancellationToken);

        if (account.DetailsSubmitted)
        {
            return null;
        }

        // Create onboarding link.
        var service = new AccountLinkService(this.client);
        var request = new AccountLinkCreateOptions()
        {
            Account = method.AccountId,
            RefreshUrl = $"{returnUri.AbsoluteUri}?setup-expired={method.Id}",
            ReturnUrl = returnUri.AbsoluteUri,
            Type = "account_onboarding",
        };

        var link = await service.CreateAsync(request, null, cancellationToken);

        return new Uri(link.Url, UriKind.Absolute);
    }

    public override async Task<PaymentMethodStatus> GetStatusAsync(PaymentMethod method, CancellationToken cancellationToken = default)
    {
        PaymentIntent intent;

        if (method is FullPaymentMethod f)
        {
            intent = f.Data;
        }
        else
        {
            var service = new PaymentIntentService(this.client);

            intent = await service.GetAsync(method.Id, null, null, cancellationToken);
        }

        switch (intent.Status)
        {
            case "requires_payment_method":
            case "requires_confirmation":
            case "requires_action":
            case "processing":
            case "requires_capture":
                return PaymentMethodStatus.Created;
            case "succeeded":
                return PaymentMethodStatus.Succeeded;
            case "canceled":
                return PaymentMethodStatus.Canceled;
            default:
                throw new NotImplementedException($"Status {intent.Status} is not implemented.");
        }
    }

    public override async Task<ReceivingMethodStatus> GetStatusAsync(ReceivingMethod method, CancellationToken cancellationToken)
    {
        var account = await this.GetAccountAsync(method.AccountId, cancellationToken);

        if (!account.DetailsSubmitted)
        {
            return ReceivingMethodStatus.SetupRequired;
        }
        else if (!account.ChargesEnabled)
        {
            return ReceivingMethodStatus.Processing;
        }
        else
        {
            return ReceivingMethodStatus.Ready;
        }
    }

    private Task<Account> GetAccountAsync(string id, CancellationToken cancellationToken = default)
    {
        var service = new AccountService(this.client);

        return service.GetAsync(id, null, null, cancellationToken);
    }
}
