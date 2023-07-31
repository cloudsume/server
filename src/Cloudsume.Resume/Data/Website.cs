namespace Cloudsume.Resume.Data;

using System;
using Candidate.Server.Resume;

[ResumeData(StaticType)]
public sealed class Website : ResumeData
{
    public const string StaticType = "website";
    public const string ValueProperty = "url";

    public Website(DataProperty<Uri?> value, DateTime updatedAt)
        : base(updatedAt)
    {
        this.Value = value;
    }

    public override string Type => StaticType;

    [DataProperty(ValueProperty)]
    public DataProperty<Uri?> Value { get; }
}
