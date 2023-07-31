namespace Cloudsume.Builder.AttributeFactories;

using Cloudsume.Resume.Data;

internal sealed class CertificateAttributeFactory : ListAttributeFactory<Certificate>
{
    protected override object? Create(BuildContext context, Certificate data) => new
    {
        Name = TexString.From(data.Name.Value),
        ObtainedDate = GetDate(data.Obtained.Value),
    };
}
