namespace Cloudsume.Resume.Data;

using System;
using Candidate.Server.Resume;
using Ultima.Extensions.Primitives;

[ResumeData(StaticType)]
public sealed class Certificate : MultiplicativeData
{
    public const string StaticType = "certificate";
    public const string NameProperty = "name";
    public const string ObtainedProperty = "obtained";

    public Certificate(Guid? id, Guid? baseId, DataProperty<string?> name, DataProperty<YearMonth?> obtained, DateTime updatedAt)
        : base(id, baseId, updatedAt)
    {
        this.Name = name;
        this.Obtained = obtained;
    }

    public override string Type => StaticType;

    [DataProperty(NameProperty)]
    public DataProperty<string?> Name { get; }

    [DataProperty(ObtainedProperty)]
    public DataProperty<YearMonth?> Obtained { get; }
}
