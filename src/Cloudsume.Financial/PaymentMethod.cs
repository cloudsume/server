namespace Cloudsume.Financial
{
    public abstract class PaymentMethod
    {
        /// <summary>
        /// Gets a value indicating whether this instance contain only a reference to a payment method.
        /// </summary>
        public abstract bool IsReference { get; }
    }
}
