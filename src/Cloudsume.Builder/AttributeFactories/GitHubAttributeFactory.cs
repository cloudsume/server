namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class GitHubAttributeFactory : UniqueAttributeFactory<GitHub>
{
    protected override object? Create(BuildContext context, GitHub data)
    {
        return TexString.From(data.Username.Value);
    }
}
