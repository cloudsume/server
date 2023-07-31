namespace Cloudsume.Options
{
    using Ultima.Extensions.DataValidation;

    public sealed class TemplatePriceConstraint
    {
        [NonNegative]
        public decimal Min { get; set; }

        [NonNegative]
        public decimal Max { get; set; }
    }
}
