namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Builder;
using Candidate.Server.Resume.Data;

internal sealed class ExperienceAttributeFactory : ListAttributeFactory<Experience>
{
    protected override object? Create(BuildContext context, Experience data) => new
    {
        Start = GetDate(data.Start.Value),
        End = GetDate(data.End.Value),
        Title = TexString.From(data.Title.Value),
        Company = TexString.From(data.Company.Value),
        Country = data.Region.Value is { } r ? GetCountry(context, r.Country) : null,
        Region = data.Region.Value,
        Street = TexString.From(data.Street.Value),
        Description = ExperienceDescription.From(data.Description.Value),
    };
}
