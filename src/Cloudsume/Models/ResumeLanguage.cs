namespace Cloudsume.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Server.Models;

    public sealed class ResumeLanguage : MultiplicativeData
    {
        [JsonConstructor]
        public ResumeLanguage(
            Guid id,
            Guid? @base,
            DataProperty<string?> tag,
            DataProperty<ResumeLanguageProficiency?> proficiency,
            DataProperty<string?> comment)
            : base(id, @base)
        {
            this.Tag = tag.AddValidator(new AcceptAttribute("en", "hi", "th"));
            this.Proficiency = proficiency;
            this.Comment = comment.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
        }

        public ResumeLanguage(Language domain)
            : base(domain)
        {
            this.Tag = DataProperty.From(domain.Value, v => v?.Name);
            this.Proficiency = DataProperty.From(domain.Proficiency, v => v is { } p ? new ResumeLanguageProficiency(p) : null);
            this.Comment = new(domain.Comment);
        }

        public DataProperty<string?> Tag { get; set; }

        public DataProperty<ResumeLanguageProficiency?> Proficiency { get; set; }

        public DataProperty<string?> Comment { get; set; }
    }
}
