namespace Cloudsume.Builder.AttributeFactories;

using Cloudsume.Builder.Values;
using Domain = Candidate.Server.Resume.Data.Language;

internal sealed class LanguageAttributeFactory : ListAttributeFactory<Domain>
{
    protected override object? Create(BuildContext context, Domain data) => new
    {
        Name = Language.From(data.Value.Value),
        Proficiency = data.Proficiency.Value,
        Comment = TexString.From(data.Comment.Value),
    };
}
