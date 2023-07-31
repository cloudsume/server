namespace Cloudsume.Financial
{
    using System;
    using System.Security.Claims;
    using Ultima.Extensions.Currency;

    public sealed class PaymentInfo
    {
        public PaymentInfo(ClaimsPrincipal payer, CurrencyCode currency, decimal amount, string description)
        {
            if (amount <= 0m || !CurrencyInfo.Get(currency).IsValidAmount(amount))
            {
                throw new ArgumentException($"The value is not valid amount.", nameof(amount));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("The value is empty.", nameof(description));
            }

            this.Payer = payer;
            this.Currency = currency;
            this.Amount = amount;
            this.Description = description;
        }

        public ClaimsPrincipal Payer { get; }

        public CurrencyCode Currency { get; }

        public decimal Amount { get; }

        public string Description { get; }
    }
}
