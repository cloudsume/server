namespace Candidate.Server.Resume.Data;

using System;
using System.Globalization;
using Cloudsume.Resume;

[ResumeData(StaticType)]
public sealed class Language : MultiplicativeData
{
    public const string StaticType = "language";
    public const string ValueProperty = "language";
    public const string ProficiencyProperty = "proficiency";
    public const string CommentProperty = "comment";

    public Language(
        Guid? id,
        Guid? baseId,
        DataProperty<CultureInfo?> value,
        DataProperty<LanguageProficiency?> proficiency,
        DataProperty<string?> comment,
        DateTime updatedAt)
        : base(id, baseId, updatedAt)
    {
        this.Value = value;
        this.Proficiency = proficiency;
        this.Comment = comment;
    }

    public override sealed string Type => StaticType;

    [DataProperty(ValueProperty)]
    public DataProperty<CultureInfo?> Value { get; }

    [DataProperty(ProficiencyProperty)]
    public DataProperty<LanguageProficiency?> Proficiency { get; }

    [DataProperty(CommentProperty)]
    public DataProperty<string?> Comment { get; }
}
