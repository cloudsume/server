namespace Cloudsume.Models
{
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using static Candidate.Server.Resume.Data.SkillRenderOptions;
    using Domain = Candidate.Server.Resume.Data.SkillRenderOptions;

    public sealed class SkillOptions
    {
        public SkillOptions(Domain domain)
        {
            this.Grouping = domain.Grouping;
        }

        [JsonConstructor]
        public SkillOptions(GroupingType grouping)
        {
            this.Grouping = grouping;
        }

        [RequireDefined]
        public GroupingType Grouping { get; }
    }
}
