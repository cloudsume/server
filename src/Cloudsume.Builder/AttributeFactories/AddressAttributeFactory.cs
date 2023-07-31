namespace Cloudsume.Builder.AttributeFactories;

using Cloudsume.Resume.Data;

internal sealed class AddressAttributeFactory : UniqueAttributeFactory<Address>
{
    protected override object? Create(BuildContext context, Address data)
    {
        var region = data.Region.Value;

        return new
        {
            Country = region is { } r ? GetCountry(context, r.Country) : null,
            Region = region,
            Street = TexString.From(data.Street.Value),
        };
    }
}
