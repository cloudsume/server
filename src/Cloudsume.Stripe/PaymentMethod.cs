namespace Cloudsume.Stripe
{
    public class PaymentMethod : Cloudsume.Financial.PaymentMethod
    {
        public PaymentMethod(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets a unique identifier of payment intent that associated with this method.
        /// </summary>
        public string Id { get; }

        public override bool IsReference => true;
    }
}
