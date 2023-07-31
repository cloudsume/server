namespace Cloudsume.Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ResumeIeltsScoreAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value switch
            {
                decimal v => v >= 0m && v <= 9m && (v % 0.5m) == 0m,
                _ => true,
            };
        }
    }
}
