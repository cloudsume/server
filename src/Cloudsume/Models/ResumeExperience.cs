namespace Cloudsume.Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Candidate.Server.Resume.Data;
    using Ultima.Extensions.Primitives;
    using SubdivisionCode = Ultima.Extensions.Globalization.SubdivisionCode;

    public sealed class ResumeExperience : MultiplicativeData
    {
        [JsonConstructor]
        public ResumeExperience(
            Guid id,
            Guid? @base,
            DataProperty<YearMonth?> start,
            DataProperty<YearMonth?> end,
            DataProperty<string?> title,
            DataProperty<string?> company,
            DataProperty<SubdivisionCode?> region,
            DataProperty<string?> street,
            DataProperty<string?> description)
            : base(id, @base)
        {
            this.Start = start;
            this.End = end;
            this.Title = title.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
            this.Company = company.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
            this.Region = region;
            this.Street = street.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
            this.Description = description.AddValidator(new StringLengthAttribute(1000) { MinimumLength = 1 });
        }

        public ResumeExperience(Experience domain)
            : base(domain)
        {
            this.Start = new(domain.Start);
            this.End = new(domain.End);
            this.Title = new(domain.Title);
            this.Company = new(domain.Company);
            this.Region = new(domain.Region);
            this.Street = new(domain.Street);
            this.Description = new(domain.Description);
        }

        public DataProperty<YearMonth?> Start { get; }

        public DataProperty<YearMonth?> End { get; }

        public DataProperty<string?> Title { get; }

        public DataProperty<string?> Company { get; }

        public DataProperty<SubdivisionCode?> Region { get; }

        public DataProperty<string?> Street { get; }

        public DataProperty<string?> Description { get; }
    }
}
