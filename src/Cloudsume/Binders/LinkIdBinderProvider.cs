namespace Cloudsume.Binders
{
    using Cloudsume.Resume;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    internal sealed class LinkIdBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.UnderlyingOrModelType == typeof(LinkId))
            {
                return new LinkIdBinder();
            }

            return null;
        }
    }
}
