namespace Cloudsume.Server.Models
{
    using System;
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Resume.Data;

    [JsonConverter(typeof(ResumeLanguageProficiencyConverter))]
    public sealed class ResumeLanguageProficiency
    {
        public ResumeLanguageProficiency(LanguageProficiency domain)
        {
            switch (domain)
            {
                case ILR v:
                    this.Type = ResumeLanguageProficiencyType.ILR;
                    this.Value = v.Scale;
                    break;
                case TOEIC v:
                    this.Type = ResumeLanguageProficiencyType.TOEIC;
                    this.Value = v.Score;
                    break;
                case IELTS v:
                    this.Type = ResumeLanguageProficiencyType.IELTS;
                    this.Value = new ResumeIelts(v);
                    break;
                case TOEFL v:
                    this.Type = ResumeLanguageProficiencyType.TOEFL;
                    this.Value = v.Score;
                    break;
                default:
                    throw new ArgumentException($"Unknow proficiency {domain.GetType()}.", nameof(domain));
            }
        }

        public ResumeLanguageProficiency(ResumeLanguageProficiencyType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        [RequireDefined]
        public ResumeLanguageProficiencyType Type { get; set; }

        [ResumeLanguageProficiencyValue]
        public object Value { get; set; }
    }
}
