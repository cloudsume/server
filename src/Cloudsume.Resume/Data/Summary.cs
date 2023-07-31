namespace Candidate.Server.Resume.Data
{
    using System;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class Summary : ResumeData
    {
        public const string StaticType = "summary";
        public const string ValueProperty = "summary";

        public Summary(DataProperty<string?> value, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Value = value;
        }

        public override string Type => StaticType;

        [DataProperty(ValueProperty)]
        public DataProperty<string?> Value { get; }
    }
}
