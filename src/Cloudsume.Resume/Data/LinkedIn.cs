namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class LinkedIn : ResumeData
    {
        public const string StaticType = "linkedin";
        public const string UsernameProperty = "username";

        public LinkedIn(DataProperty<string?> username, DateTime updatedAt)
            : base(updatedAt)
        {
            if (username.Value?.Length == 0)
            {
                throw new ArgumentException("The value is empty.", nameof(username));
            }

            this.Username = username;
        }

        public override string Type => StaticType;

        [DataProperty(UsernameProperty)]
        public DataProperty<string?> Username { get; }
    }
}
