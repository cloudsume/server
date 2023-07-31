namespace Cloudsume.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cloudsume.Server.Models;
using Ultima.Extensions.Primitives;
using Domain = Cloudsume.Resume.Data.Certificate;

public sealed class ResumeCertificate : MultiplicativeData
{
    public ResumeCertificate(Domain domain)
        : base(domain)
    {
        this.Name = new(domain.Name);
        this.Obtained = new(domain.Obtained);
    }

    [JsonConstructor]
    public ResumeCertificate(Guid id, Guid? @base, DataProperty<string?> name, DataProperty<YearMonth?> obtained)
        : base(id, @base)
    {
        this.Name = name.AddValidator(new StringLengthAttribute(100) { MinimumLength = 1 });
        this.Obtained = obtained;
    }

    public DataProperty<string?> Name { get; }

    public DataProperty<YearMonth?> Obtained { get; }
}
