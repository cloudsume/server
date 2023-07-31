namespace Candidate.Server
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MemberValidationAttribute : ValidationAttribute
    {
        protected MemberValidationAttribute()
        {
        }

        public override bool RequiresValidationContext => true;

        [return: MaybeNull]
        protected T GetPropertyValue<T>(string name, ValidationContext context)
        {
            return (T)this.GetPropertyValue(name, context)!;
        }

        protected object? GetPropertyValue(string name, ValidationContext context)
        {
            var type = context.ObjectType;
            var property = type.GetProperty(name);

            if (property == null)
            {
                throw new ArgumentException($"'{name}' is not a valid property on {type}.", nameof(name));
            }

            return property.GetValue(context.ObjectInstance);
        }
    }
}
