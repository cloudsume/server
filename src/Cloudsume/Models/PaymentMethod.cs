namespace Cloudsume.Models;

using System;
using Domain = Cloudsume.Financial.PaymentMethod;

public abstract class PaymentMethod
{
    protected PaymentMethod(Domain domain, decimal amount)
    {
        this.Amount = amount;
    }

    public abstract PaymentProvider Provider { get; }

    public decimal Amount { get; }

    public static PaymentMethod From(Domain domain, decimal amount) => domain switch
    {
        Cloudsume.Stripe.FullPaymentMethod m => new StripePayment(m, amount),
        _ => throw new ArgumentException($"Unknown payment method {domain.GetType()}.", nameof(domain)),
    };
}
