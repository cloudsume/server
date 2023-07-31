namespace Cloudsume.Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Candidate.Server;
    using static Candidate.Server.Resume.Data.ILR;

    public sealed class ResumeLanguageProficiencyValueAttribute : MemberValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var type = this.GetPropertyValue<ResumeLanguageProficiencyType>(nameof(ResumeLanguageProficiency.Type), validationContext);

            return type switch
            {
                ResumeLanguageProficiencyType.ILR => ValidateIlr(),
                ResumeLanguageProficiencyType.TOEIC => ValidateToeic(),
                ResumeLanguageProficiencyType.IELTS => ValidateIelts(),
                ResumeLanguageProficiencyType.TOEFL => ValidateToefl(),
                _ => null,
            };

            ValidationResult? ValidateIlr() => value is ScaleId v && Enum.IsDefined(v) ? null : new("Invalid ILR scale.");
            ValidationResult? ValidateToeic() => value is int v && v >= 0 && v <= 990 ? null : new("Invalid TOEIC score.");
            ValidationResult? ValidateIelts() => value is ResumeIelts ? null : new("Invalid IELTS score.");
            ValidationResult? ValidateToefl() => value is int v && v >= 0 && v <= 120 ? null : new("Invalid TOEFL score.");
        }
    }
}
