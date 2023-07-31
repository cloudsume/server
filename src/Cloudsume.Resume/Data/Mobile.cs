namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;
    using Ultima.Extensions.Telephony;

    [ResumeData(StaticType)]
    public sealed class Mobile : ResumeData
    {
        public const string StaticType = "mobile";
        public const string ValueProperty = "number";

        public Mobile(DataProperty<TelephoneNumber?> value, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Value = value;
        }

        public override string Type => StaticType;

        [DataProperty(ValueProperty)]
        public DataProperty<TelephoneNumber?> Value { get; }
    }
}
