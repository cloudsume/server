namespace Cloudsume.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Cloudsume.Server.Models;
    using Ultima.Extensions.Globalization;
    using Ultima.Extensions.Primitives;
    using Domain = Candidate.Server.Resume.Data.Education;

    public sealed class ResumeEducation : MultiplicativeData
    {
        [JsonConstructor]
        public ResumeEducation(
            Guid id,
            Guid? @base,
            DataProperty<string?>? institute,
            DataProperty<SubdivisionCode?>? region,
            DataProperty<string?>? degreeName,
            DataProperty<YearMonth?> start,
            DataProperty<YearMonth?> end,
            DataProperty<string?> grade,
            DataProperty<string?> description)
            : base(id, @base)
        {
            this.Institute = institute?.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
            this.Region = region;
            this.DegreeName = degreeName?.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
            this.Start = start;
            this.End = end;
            this.Grade = grade.AddValidator(new StringLengthAttribute(30) { MinimumLength = 1 });
            this.Description = description.AddValidator(new StringLengthAttribute(1000) { MinimumLength = 1 });
        }

        public ResumeEducation(Domain domain)
            : base(domain)
        {
            this.Institute = new(domain.Institute);
            this.Region = new(domain.Region);
            this.DegreeName = new(domain.DegreeName);
            this.Start = new(domain.Start);
            this.End = new(domain.End);
            this.Grade = new(domain.Grade);
            this.Description = new(domain.Description);
        }

        public DataProperty<string?>? Institute { get; }

        public DataProperty<SubdivisionCode?>? Region { get; }

        public DataProperty<string?>? DegreeName { get; }

        public DataProperty<YearMonth?> Start { get; }

        public DataProperty<YearMonth?> End { get; }

        public DataProperty<string?> Grade { get; }

        public DataProperty<string?> Description { get; }
    }
}
