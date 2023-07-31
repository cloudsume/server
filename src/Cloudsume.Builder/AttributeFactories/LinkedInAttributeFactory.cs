namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class LinkedInAttributeFactory : UniqueAttributeFactory<LinkedIn>
{
    protected override object? Create(BuildContext context, LinkedIn data)
    {
        return TexString.From(data.Username.Value);
    }
}
