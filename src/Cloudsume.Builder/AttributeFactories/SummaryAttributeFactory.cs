namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class SummaryAttributeFactory : UniqueAttributeFactory<Summary>
{
    protected override object? Create(BuildContext context, Summary data)
    {
        return TexString.From(data.Value.Value);
    }
}
