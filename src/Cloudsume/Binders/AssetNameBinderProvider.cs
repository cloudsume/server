namespace Cloudsume.Binders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using AssetName = Cloudsume.Resume.AssetName;

internal sealed class AssetNameBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.UnderlyingOrModelType == typeof(AssetName))
        {
            return new AssetNameBinder();
        }

        return null;
    }
}
