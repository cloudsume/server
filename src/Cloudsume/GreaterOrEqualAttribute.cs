namespace Candidate.Server
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public sealed class GreaterOrEqualAttribute : MemberValidationAttribute
    {
        private readonly string otherProperty;

        public GreaterOrEqualAttribute(string otherProperty)
        {
            this.otherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var other = this.GetPropertyValue(this.otherProperty, validationContext);

            if (((IComparable)value).CompareTo(other) < 0)
            {
                return new ValidationResult(
                    $"{validationContext.DisplayName} is less than {this.otherProperty}.",
                    validationContext.MemberName != null ? new[] { validationContext.MemberName } : null);
            }
            else
            {
                return null;
            }
        }
    }
}
