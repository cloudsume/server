namespace Cloudsume.Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using FromClient = Cloudsume.Resume.DataSources.FromClient;
    using PropertyFlags = Cloudsume.Resume.PropertyFlags;

    public static class DataProperty
    {
        public static DataProperty<TModel> From<TModel, TDomain>(Cloudsume.Resume.DataProperty<TDomain> domain, Func<TDomain?, TModel> mapper)
        {
            var flags = domain.Flags;
            var value = mapper(domain.Value);

            return new(flags, value);
        }
    }

    public sealed class DataProperty<T> : IValidatableObject
    {
        private readonly List<ValidationAttribute> validators;

        [JsonConstructor]
        public DataProperty(PropertyFlags flags, T? value)
        {
            this.Flags = flags;
            this.Value = value;
            this.validators = new(4);
        }

        public DataProperty(Cloudsume.Resume.DataProperty<T> domain)
        {
            this.Flags = domain.Flags;
            this.Value = domain.Value;
            this.validators = new();
        }

        [RequireDefined]
        public PropertyFlags Flags { get; }

        public T? Value { get; }

        public static implicit operator Cloudsume.Resume.DataProperty<T?>(DataProperty<T?>? dto)
        {
            return dto is null ? new(default, new FromClient()) : dto.ToDomain();
        }

        public DataProperty<T> AddValidator(ValidationAttribute validator)
        {
            this.validators.Add(validator);
            return this;
        }

        public Cloudsume.Resume.DataProperty<T?> ToDomain()
        {
            return new(this.Value, new FromClient(), this.Flags);
        }

        public Cloudsume.Resume.DataProperty<TDomain?> ToDomain<TDomain>(Func<T?, TDomain?> mapper)
        {
            return new(mapper(this.Value), new FromClient(), this.Flags);
        }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            foreach (var validator in this.validators)
            {
                var result = validator.GetValidationResult(this.Value, validationContext);

                if (result != null)
                {
                    yield return result;
                }
            }
        }
    }
}
