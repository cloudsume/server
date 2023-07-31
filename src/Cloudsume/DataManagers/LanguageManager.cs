namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Cloudsume.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.Language;

internal sealed class LanguageManager : ListDataManager<Domain, ResumeLanguage>
{
    public LanguageManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override int MaxLocal => 5;

    public override int MaxGlobal => 20;

    public override Task<SampleUpdate<ResumeLanguage>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<ResumeLanguage>>(data, cancellationToken);
    }

    public override Task<ResumeLanguage> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<ResumeLanguage>(data, cancellationToken);
    }

    public override Domain ToDomain(ResumeLanguage dto, IReadOnlyDictionary<string, object> contents)
    {
        // Get language identifier.
        var language = dto.Tag.ToDomain<CultureInfo?>(t =>
        {
            if (t == null)
            {
                return null;
            }

            try
            {
                return CultureInfo.GetCultureInfo(t);
            }
            catch (CultureNotFoundException ex)
            {
                throw new DataUpdateException("Invalid tag.", ex);
            }
        });

        // Get language proficiency.
        var proficiency = dto.Proficiency.ToDomain<Candidate.Server.Resume.Data.LanguageProficiency?>(p =>
        {
            if (p == null)
            {
                return null;
            }

            return p.Type switch
            {
                ResumeLanguageProficiencyType.ILR => new Candidate.Server.Resume.Data.ILR((Candidate.Server.Resume.Data.ILR.ScaleId)p.Value),
                ResumeLanguageProficiencyType.TOEIC => new Candidate.Server.Resume.Data.TOEIC((int)p.Value),
                ResumeLanguageProficiencyType.IELTS => GetIELTS(),
                ResumeLanguageProficiencyType.TOEFL => new Cloudsume.Resume.Data.TOEFL((int)p.Value),
                _ => throw new DataUpdateException("Invalid proficiency type."),
            };

            Candidate.Server.Resume.Data.IELTS GetIELTS()
            {
                var value = (ResumeIelts)p.Value;
                return new(value.Type, value.BandScore);
            }
        });

        return new(dto.Id, dto.Base, language, proficiency, dto.Comment, DateTime.Now);
    }

    public override ResumeLanguage ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain);
    }
}
