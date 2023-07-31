namespace Cloudsume.Stripe
{
    using System.ComponentModel.DataAnnotations;

    public sealed class StripeOptions
    {
        [Required]
        public string? Key { get; set; }
    }
}
