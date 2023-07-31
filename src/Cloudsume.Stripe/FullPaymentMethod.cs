namespace Cloudsume.Stripe
{
    using global::Stripe;

    public sealed class FullPaymentMethod : PaymentMethod
    {
        public FullPaymentMethod(PaymentIntent intent)
            : base(intent.Id)
        {
            this.Data = intent;
        }

        public PaymentIntent Data { get; }

        public override bool IsReference => false;
    }
}
