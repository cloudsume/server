namespace Candidate.Server.Resume.Data
{
    using System;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class Name : ResumeData
    {
        public const string StaticType = "name";
        public const string FirstNameProperty = "first";
        public const string MiddleNameProperty = "middle";
        public const string LastNameProperty = "last";

        public Name(DataProperty<string?> firstName, DataProperty<string?> middleName, DataProperty<string?> lastName, DateTime updatedAt)
            : base(updatedAt)
        {
            this.FirstName = firstName;
            this.MiddleName = middleName;
            this.LastName = lastName;
        }

        public override string Type => StaticType;

        [DataProperty(FirstNameProperty)]
        public DataProperty<string?> FirstName { get; }

        [DataProperty(MiddleNameProperty)]
        public DataProperty<string?> MiddleName { get; }

        [DataProperty(LastNameProperty)]
        public DataProperty<string?> LastName { get; }
    }
}
