namespace Cloudsume.Builder.AttributeFactories;

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume.Data;
using Ultima.Extensions.Graphics;

internal sealed class PhotoAttributeFactory : UniqueAttributeFactory<Photo>
{
    private static readonly TypeConverter ImageFormatConverter = TypeDescriptor.GetConverter(typeof(ImageFormat));

    public override async ValueTask<object?> CreateAsync(BuildContext context, Photo data, CancellationToken cancellationToken = default)
    {
        var info = data.Info.Value;

        if (info == null)
        {
            return null;
        }

        var name = Guid.NewGuid().ToString();
        var extension = ImageFormatConverter.ConvertToInvariantString(info.Format);
        await using var image = await data.GetImageAsync(cancellationToken);

        await context.Job.WriteAssetAsync($"{name}.{extension}", info.Size, image, cancellationToken);

        return name;
    }
}
