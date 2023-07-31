namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class HeadlineAttributeFactory : UniqueAttributeFactory<Headline>
{
    protected override object? Create(BuildContext context, Headline data)
    {
        return TexString.From(data.Value.Value);
    }
}
