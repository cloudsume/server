namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;
    using Ultima.Extensions.Globalization;
    using Ultima.Extensions.Primitives;

    [ResumeData(StaticType)]
    public sealed class Education : MultiplicativeData
    {
        public const string StaticType = "education";
        public const string InstituteProperty = "institute";
        public const string RegionProperty = "region";
        public const string DegreeNameProperty = "degree";
        public const string StartProperty = "start";
        public const string EndProperty = "end";
        public const string GradeProperty = "grade";
        public const string DescriptionProperty = "description";

        public Education(
            Guid? id,
            Guid? baseId,
            DataProperty<string?> institute,
            DataProperty<SubdivisionCode?> region,
            DataProperty<string?> degreeName,
            DataProperty<YearMonth?> start,
            DataProperty<YearMonth?> end,
            DataProperty<string?> grade,
            DataProperty<string?> description,
            DateTime updatedAt)
            : base(id, baseId, updatedAt)
        {
            this.Institute = institute;
            this.Region = region;
            this.DegreeName = degreeName;
            this.Start = start;
            this.End = end;
            this.Grade = grade;
            this.Description = description;
        }

        public override string Type => StaticType;

        [DataProperty(InstituteProperty)]
        public DataProperty<string?> Institute { get; }

        [DataProperty(RegionProperty)]
        public DataProperty<SubdivisionCode?> Region { get; }

        [DataProperty(DegreeNameProperty)]
        public DataProperty<string?> DegreeName { get; }

        [DataProperty(StartProperty)]
        public DataProperty<YearMonth?> Start { get; }

        [DataProperty(EndProperty)]
        public DataProperty<YearMonth?> End { get; }

        [DataProperty(GradeProperty)]
        public DataProperty<string?> Grade { get; }

        [DataProperty(DescriptionProperty)]
        public DataProperty<string?> Description { get; }
    }
}
