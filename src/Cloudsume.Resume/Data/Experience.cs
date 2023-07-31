namespace Candidate.Server.Resume.Data
{
    using System;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;
    using Ultima.Extensions.Globalization;
    using Ultima.Extensions.Primitives;

    [ResumeData(StaticType)]
    public sealed class Experience : MultiplicativeData
    {
        public const string StaticType = "experience";
        public const string StartProperty = "start";
        public const string EndProperty = "end";
        public const string TitleProperty = "title";
        public const string CompanyProperty = "company";
        public const string RegionProperty = "region";
        public const string StreetProperty = "street";
        public const string DescriptionProperty = "description";

        public Experience(
            Guid? id,
            Guid? baseId,
            DataProperty<YearMonth?> start,
            DataProperty<YearMonth?> end,
            DataProperty<string?> title,
            DataProperty<string?> company,
            DataProperty<SubdivisionCode?> region,
            DataProperty<string?> street,
            DataProperty<string?> description,
            DateTime updatedAt)
            : base(id, baseId, updatedAt)
        {
            this.Start = start;
            this.End = end;
            this.Title = title;
            this.Company = company;
            this.Region = region;
            this.Street = street;
            this.Description = description;
        }

        public override string Type => StaticType;

        [DataProperty(StartProperty)]
        public DataProperty<YearMonth?> Start { get; }

        [DataProperty(EndProperty)]
        public DataProperty<YearMonth?> End { get; }

        [DataProperty(TitleProperty)]
        public DataProperty<string?> Title { get; }

        [DataProperty(CompanyProperty)]
        public DataProperty<string?> Company { get; }

        [DataProperty(RegionProperty)]
        public DataProperty<SubdivisionCode?> Region { get; }

        [DataProperty(StreetProperty)]
        public DataProperty<string?> Street { get; }

        [DataProperty(DescriptionProperty)]
        public DataProperty<string?> Description { get; }
    }
}
