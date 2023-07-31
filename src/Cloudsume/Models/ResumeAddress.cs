namespace Cloudsume.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Cloudsume.Server.Models;
    using Domain = Cloudsume.Resume.Data.Address;
    using SubdivisionCode = Ultima.Extensions.Globalization.SubdivisionCode;

    public sealed class ResumeAddress
    {
        [JsonConstructor]
        public ResumeAddress(DataProperty<SubdivisionCode?> region, DataProperty<string?> street)
        {
            this.Region = region;
            this.Street = street.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
        }

        public ResumeAddress(Domain domain)
        {
            this.Region = new(domain.Region);
            this.Street = new(domain.Street);
        }

        public DataProperty<SubdivisionCode?> Region { get; }

        public DataProperty<string?> Street { get; }
    }
}
