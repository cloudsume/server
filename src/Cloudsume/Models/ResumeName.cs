namespace Cloudsume.Server.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Candidate.Server.Resume.Data;

    public sealed class ResumeName
    {
        [JsonConstructor]
        public ResumeName(DataProperty<string?> firstName, DataProperty<string?> middleName, DataProperty<string?> lastName)
        {
            var validator = new StringLengthAttribute(100) { MinimumLength = 1 };

            this.FirstName = firstName.AddValidator(validator);
            this.MiddleName = middleName.AddValidator(validator);
            this.LastName = lastName.AddValidator(validator);
        }

        public ResumeName(Name domain)
        {
            this.FirstName = new(domain.FirstName);
            this.MiddleName = new(domain.MiddleName);
            this.LastName = new(domain.LastName);
        }

        public DataProperty<string?> FirstName { get; }

        public DataProperty<string?> MiddleName { get; }

        public DataProperty<string?> LastName { get; }
    }
}
