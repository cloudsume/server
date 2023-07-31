namespace Cloudsume.Models;

using Domain = Cloudsume.Stripe.FullPaymentMethod;

public sealed class StripePayment : PaymentMethod
{
    public StripePayment(Domain domain, decimal amount)
        : base(domain, amount)
    {
        this.ClientSecret = domain.Data.ClientSecret;
    }

    public override PaymentProvider Provider => PaymentProvider.Stripe;

    public string ClientSecret { get; }
}
