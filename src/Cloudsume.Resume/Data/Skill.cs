namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class Skill : MultiplicativeData
    {
        public const string StaticType = "skill";
        public const string NameProperty = "name";
        public const string LevelProperty = "proficiency";

        public Skill(Guid? id, Guid? baseId, DataProperty<string?> name, DataProperty<SkillLevel?> level, DateTime updatedAt)
            : base(id, baseId, updatedAt)
        {
            this.Name = name;
            this.Level = level;
        }

        public override string Type => StaticType;

        [DataProperty(NameProperty)]
        public DataProperty<string?> Name { get; }

        [DataProperty(LevelProperty)]
        public DataProperty<SkillLevel?> Level { get; }
    }
}
