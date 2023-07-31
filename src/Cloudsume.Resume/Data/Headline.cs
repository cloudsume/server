namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class Headline : ResumeData
    {
        public const string StaticType = "headline";
        public const string ValueProperty = "headline";

        public Headline(DataProperty<string?> value, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Value = value;
        }

        public override string Type => StaticType;

        [DataProperty(ValueProperty)]
        public DataProperty<string?> Value { get; }
    }
}
