namespace Candidate.Server.Resume.Data
{
    using System;
    using System.Net.Mail;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class EmailAddress : ResumeData
    {
        public const string StaticType = "email";
        public const string ValueProperty = "address";

        public EmailAddress(DataProperty<MailAddress?> value, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Value = value;
        }

        public override string Type => StaticType;

        [DataProperty(ValueProperty)]
        public DataProperty<MailAddress?> Value { get; }
    }
}
