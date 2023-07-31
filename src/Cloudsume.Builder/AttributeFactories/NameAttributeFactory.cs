namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class NameAttributeFactory : UniqueAttributeFactory<Name>
{
    protected override object? Create(BuildContext context, Name data) => new
    {
        FirstName = TexString.From(data.FirstName.Value),
        MiddleName = TexString.From(data.MiddleName.Value),
        LastName = TexString.From(data.LastName.Value),
    };
}
