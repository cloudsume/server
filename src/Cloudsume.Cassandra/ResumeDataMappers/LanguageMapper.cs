namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using System.Globalization;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Language;
using FromDatabase = Cloudsume.Resume.DataSources.FromDatabase;
using IELTS = Candidate.Server.Resume.Data.IELTS;
using ILR = Candidate.Server.Resume.Data.ILR;
using LanguageProficiency = Candidate.Server.Resume.Data.LanguageProficiency;
using PropertyFlags = Cloudsume.Resume.PropertyFlags;
using TOEFL = Cloudsume.Resume.Data.TOEFL;
using TOEIC = Candidate.Server.Resume.Data.TOEIC;

internal sealed class LanguageMapper : ResumeDataMapper<Domain, LanguageData>
{
    public override LanguageData ToCassandra(Domain domain)
    {
        var cassandra = new LanguageData()
        {
            Language = AsciiProperty.From(domain.Value, v => v?.Name),
            Comment = TextProperty.From(domain.Comment),
            ProficiencyFlags = Convert.ToSByte(domain.Proficiency.Flags),
            UpdatedTime = domain.UpdatedAt,
        };

        switch (domain.Proficiency.Value)
        {
            case ILR i:
                cassandra.Ilr = Convert.ToSByte(i.Scale);
                break;
            case TOEIC t:
                cassandra.Toeic = Convert.ToInt16(t.Score);
                break;
            case IELTS i:
                cassandra.Ielts = new()
                {
                    Type = Convert.ToSByte(i.Type),
                    Band = i.Band,
                };
                break;
            case TOEFL t:
                cassandra.Toefl = Convert.ToSByte(t.Score);
                break;
            case null:
                break;
            default:
                throw new ArgumentException($"Unknow proficiency {domain.Proficiency.Value.GetType()}.", nameof(domain));
        }

        return cassandra;
    }

    public override Domain ToDomain(Guid id, Guid? parent, LanguageData cassandra)
    {
        var language = cassandra.Language.ToDomain(v => v is null ? null : CultureInfo.GetCultureInfo(v));
        var comment = cassandra.Comment.ToDomain();
        var proficiencySource = new FromDatabase();
        var proficiencyFlags = (PropertyFlags)cassandra.ProficiencyFlags;
        Cloudsume.Resume.DataProperty<LanguageProficiency?> proficiency;

        if (cassandra.Ielts is { } ielts)
        {
            proficiency = new(new IELTS((IELTS.TypeId)ielts.Type, ielts.Band), proficiencySource, proficiencyFlags);
        }
        else if (cassandra.Ilr is { } ilr)
        {
            proficiency = new(new ILR((ILR.ScaleId)ilr), proficiencySource, proficiencyFlags);
        }
        else if (cassandra.Toeic is { } toeic)
        {
            proficiency = new(new TOEIC(toeic), proficiencySource, proficiencyFlags);
        }
        else if (cassandra.Toefl is { } toefl)
        {
            proficiency = new(new TOEFL(toefl), proficiencySource, proficiencyFlags);
        }
        else
        {
            proficiency = new(null, proficiencySource, proficiencyFlags);
        }

        return new(id, parent, language, proficiency, comment, cassandra.UpdatedTime.LocalDateTime);
    }
}
