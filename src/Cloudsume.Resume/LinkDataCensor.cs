namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Candidate.Server.Resume;
using Candidate.Server.Resume.Data;
using Cloudsume.Resume.Data;
using Cloudsume.Resume.DataSources;

internal sealed class LinkDataCensor : ILinkDataCensor
{
    public IEnumerable<ResumeData> Run(IEnumerable<ResumeData> data, IReadOnlySet<LinkCensorship> censorships, CultureInfo culture)
    {
        var outputs = new List<ResumeData>();

        foreach (var input in data)
        {
            ResumeData? output = input switch
            {
                Address i => this.CensorAddress(i, censorships),
                Certificate i => this.CensorCertificate(i, censorships),
                Education i => this.CensorEducation(i, censorships),
                EmailAddress i => this.CensorEmail(i, censorships),
                Experience i => this.CensorExperience(i, censorships),
                GitHub i => this.CensorGitHub(i, censorships),
                Headline i => this.CensorHeadline(i, censorships),
                Language i => this.CensorLanguage(i, censorships),
                LinkedIn i => this.CensorLinkedIn(i, censorships),
                Mobile i => this.CensorMobile(i, censorships),
                Name i => this.CensorName(i, censorships),
                Photo i => this.CensorPhoto(i, censorships),
                Skill i => this.CensorSkill(i, censorships),
                Summary i => this.CensorSummary(i, censorships),
                Website i => this.CensorWebsite(i, censorships),
                var i => throw new NotImplementedException($"No censor implementation for {i.GetType()}."),
            };

            if (output is not null)
            {
                outputs.Add(output);
            }
        }

        return outputs;
    }

    private static DataProperty<TProp> CensorProperty<TData, TProp>(
        IReadOnlySet<LinkCensorship> censorships,
        TData data,
        Expression<Func<TData, DataProperty<TProp>>> expression) where TData : ResumeData
    {
        var property = data.GetPropertyId(expression);
        var value = data.GetPropertyValue(property) ?? throw new Exception($"Property '{property}' does not exists on {typeof(TData)}.");

        if (value.HasValue && censorships.Contains(data.Type, property))
        {
            return (DataProperty<TProp>)value.WithValue(null, new FromDataCensor());
        }
        else
        {
            return (DataProperty<TProp>)value;
        }
    }

    private Address? CensorAddress(Address input, IReadOnlySet<LinkCensorship> censorships)
    {
        var region = CensorProperty(censorships, input, i => i.Region);
        var street = CensorProperty(censorships, input, i => i.Street);

        if (region.HasValue || street.HasValue)
        {
            return new(region, street, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Certificate? CensorCertificate(Certificate input, IReadOnlySet<LinkCensorship> censorships)
    {
        var name = CensorProperty(censorships, input, i => i.Name);
        var obtained = CensorProperty(censorships, input, i => i.Obtained);

        if (name.HasValue || obtained.HasValue)
        {
            return new(input.Id, input.BaseId, name, obtained, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Education? CensorEducation(Education input, IReadOnlySet<LinkCensorship> censorships)
    {
        var institute = CensorProperty(censorships, input, i => i.Institute);
        var region = CensorProperty(censorships, input, i => i.Region);
        var degree = CensorProperty(censorships, input, i => i.DegreeName);
        var start = CensorProperty(censorships, input, i => i.Start);
        var end = CensorProperty(censorships, input, i => i.End);
        var grade = CensorProperty(censorships, input, i => i.Grade);
        var description = CensorProperty(censorships, input, i => i.Description);

        if (institute.HasValue || region.HasValue || degree.HasValue || start.HasValue || end.HasValue || grade.HasValue || description.HasValue)
        {
            return new(input.Id, input.BaseId, institute, region, degree, start, end, grade, description, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private EmailAddress? CensorEmail(EmailAddress input, IReadOnlySet<LinkCensorship> censorships)
    {
        var address = CensorProperty(censorships, input, i => i.Value);

        if (address.HasValue)
        {
            return new(address, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Experience? CensorExperience(Experience input, IReadOnlySet<LinkCensorship> censorships)
    {
        var start = CensorProperty(censorships, input, i => i.Start);
        var end = CensorProperty(censorships, input, i => i.End);
        var title = CensorProperty(censorships, input, i => i.Title);
        var company = CensorProperty(censorships, input, i => i.Company);
        var region = CensorProperty(censorships, input, i => i.Region);
        var street = CensorProperty(censorships, input, i => i.Street);
        var description = CensorProperty(censorships, input, i => i.Description);

        if (start.HasValue || end.HasValue || title.HasValue || company.HasValue || region.HasValue || street.HasValue || description.HasValue)
        {
            return new(input.Id, input.BaseId, start, end, title, company, region, street, description, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private GitHub? CensorGitHub(GitHub input, IReadOnlySet<LinkCensorship> censorships)
    {
        var username = CensorProperty(censorships, input, i => i.Username);

        if (username.HasValue)
        {
            return new(username, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Headline? CensorHeadline(Headline input, IReadOnlySet<LinkCensorship> censorships)
    {
        var headline = CensorProperty(censorships, input, i => i.Value);

        if (headline.HasValue)
        {
            return new(headline, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Language? CensorLanguage(Language input, IReadOnlySet<LinkCensorship> censorships)
    {
        var language = CensorProperty(censorships, input, i => i.Value);
        var proficiency = CensorProperty(censorships, input, i => i.Proficiency);
        var comment = CensorProperty(censorships, input, i => i.Comment);

        if (language.HasValue || proficiency.HasValue || comment.HasValue)
        {
            return new(input.Id, input.BaseId, language, proficiency, comment, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private LinkedIn? CensorLinkedIn(LinkedIn input, IReadOnlySet<LinkCensorship> censorships)
    {
        var username = CensorProperty(censorships, input, i => i.Username);

        if (username.HasValue)
        {
            return new(username, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Mobile? CensorMobile(Mobile input, IReadOnlySet<LinkCensorship> censorships)
    {
        var number = CensorProperty(censorships, input, i => i.Value);

        if (number.HasValue)
        {
            return new(number, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Name? CensorName(Name input, IReadOnlySet<LinkCensorship> censorships)
    {
        var first = CensorProperty(censorships, input, i => i.FirstName);
        var middle = CensorProperty(censorships, input, i => i.MiddleName);
        var last = CensorProperty(censorships, input, i => i.LastName);

        if (first.HasValue || middle.HasValue || last.HasValue)
        {
            return new(first, middle, last, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Photo? CensorPhoto(Photo input, IReadOnlySet<LinkCensorship> censorships)
    {
        var info = CensorProperty(censorships, input, i => i.Info);

        if (info.HasValue)
        {
            return new(info, input.UpdatedAt)
            {
                Image = input.Image,
            };
        }
        else
        {
            return null;
        }
    }

    private Skill? CensorSkill(Skill input, IReadOnlySet<LinkCensorship> censorships)
    {
        var name = CensorProperty(censorships, input, i => i.Name);
        var level = CensorProperty(censorships, input, i => i.Level);

        if (name.HasValue || level.HasValue)
        {
            return new(input.Id, input.BaseId, name, level, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Summary? CensorSummary(Summary input, IReadOnlySet<LinkCensorship> censorships)
    {
        var summary = CensorProperty(censorships, input, i => i.Value);

        if (summary.HasValue)
        {
            return new(summary, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }

    private Website? CensorWebsite(Website input, IReadOnlySet<LinkCensorship> censorships)
    {
        var url = CensorProperty(censorships, input, i => i.Value);

        if (url.HasValue)
        {
            return new(url, input.UpdatedAt);
        }
        else
        {
            return null;
        }
    }
}
