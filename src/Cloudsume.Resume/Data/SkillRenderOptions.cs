namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    public sealed class SkillRenderOptions : TemplateRenderOptions
    {
        public SkillRenderOptions(GroupingType grouping)
        {
            this.Grouping = grouping;
        }

        public enum GroupingType
        {
            None = 0,
            Level = 1,
        }

        public override Type TargetData => typeof(Skill);

        public GroupingType Grouping { get; }
    }
}
