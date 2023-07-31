namespace Cloudsume.Builder.AttributeFactories;

using Cloudsume.Resume.Data;

internal sealed class WebsiteAttributeFactory : UniqueAttributeFactory<Website>
{
    protected override object? Create(BuildContext context, Website data)
    {
        var uri = data.Value.Value?.AbsoluteUri;

        if (uri != null && uri[^1] == '/')
        {
            return TexString.From(uri[..^1]);
        }
        else
        {
            return TexString.From(uri);
        }
    }
}
