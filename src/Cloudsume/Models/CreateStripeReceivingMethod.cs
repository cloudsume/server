namespace Cloudsume.Models
{
    using System.Globalization;

    [CorrespondingDomain(typeof(Cloudsume.Stripe.ReceivingMethod))]
    public sealed class CreateStripeReceivingMethod : CreatePaymentReceivingMethod
    {
        public CreateStripeReceivingMethod(RegionInfo country)
        {
            this.Country = country;
        }

        public override PaymentProvider Type => PaymentProvider.Stripe;

        public RegionInfo Country { get; }
    }
}
