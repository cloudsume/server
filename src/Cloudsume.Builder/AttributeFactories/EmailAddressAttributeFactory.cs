namespace Cloudsume.Builder.AttributeFactories;

using Candidate.Server.Resume.Data;

internal sealed class EmailAddressAttributeFactory : UniqueAttributeFactory<EmailAddress>
{
    protected override object? Create(BuildContext context, EmailAddress data)
    {
        return TexString.From(data.Value.Value?.Address);
    }
}
