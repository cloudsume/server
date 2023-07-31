namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;
using Ultima.Extensions.Globalization;

internal sealed class EducationAttributeFactory : ListAttributeFactory<Education>
{
    protected override object? Create(BuildContext context, Education data)
    {
        string? institute;
        SubdivisionCode? region;
        string? degree;

        // Load institute.
        if (data.Institute.Value != null)
        {
            institute = data.Institute.Value;
            region = data.Region.Value;
        }
        else
        {
            institute = null;
            region = null;
        }

        // Load degree.
        if (data.DegreeName.Value != null)
        {
            degree = data.DegreeName.Value;
        }
        else
        {
            degree = null;
        }

        return new
        {
            Start = GetDate(data.Start.Value),
            End = GetDate(data.End.Value),
            Degree = TexString.From(degree),
            Institute = TexString.From(institute),
            Country = GetCountry(context, region?.Country),
            Region = region,
            Grade = TexString.From(data.Grade.Value),
            Description = Candidate.Server.Resume.Builder.Values.EducationDescription.From(data.Description.Value),
        };
    }
}
