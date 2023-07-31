namespace Cloudsume.Resume.Data
{
    using System;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;
    using Ultima.Extensions.Globalization;

    [ResumeData(StaticType)]
    public sealed class Address : ResumeData
    {
        public const string StaticType = "address";
        public const string RegionProperty = "region";
        public const string StreetProperty = "street";

        public Address(DataProperty<SubdivisionCode?> region, DataProperty<string?> street, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Region = region;
            this.Street = street;
        }

        public override string Type => StaticType;

        [DataProperty(RegionProperty)]
        public DataProperty<SubdivisionCode?> Region { get; }

        [DataProperty(StreetProperty)]
        public DataProperty<string?> Street { get; }
    }
}
