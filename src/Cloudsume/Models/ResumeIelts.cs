namespace Cloudsume.Server.Models
{
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using Candidate.Server.Resume.Data;
    using static Candidate.Server.Resume.Data.IELTS;

    public sealed class ResumeIelts
    {
        [JsonConstructor]
        public ResumeIelts()
        {
        }

        public ResumeIelts(IELTS domain)
        {
            this.Type = domain.Type;
            this.BandScore = domain.Band;
        }

        [RequireDefined]
        public TypeId Type { get; set; }

        [ResumeIeltsScore]
        public decimal BandScore { get; set; }
    }
}
