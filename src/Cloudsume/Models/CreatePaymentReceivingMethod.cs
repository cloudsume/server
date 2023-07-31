namespace Cloudsume.Models
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(CreatePaymentReceivingMethodConverter))]
    public abstract class CreatePaymentReceivingMethod
    {
        public abstract PaymentProvider Type { get; }
    }
}
