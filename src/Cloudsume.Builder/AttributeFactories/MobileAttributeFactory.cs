namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class MobileAttributeFactory : UniqueAttributeFactory<Mobile>
{
    protected override object? Create(BuildContext context, Mobile data)
    {
        return data.Value.Value;
    }
}
