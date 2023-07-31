namespace Cloudsume.Binders
{
    using System;
    using System.Threading.Tasks;
    using Cloudsume.Resume;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.WebUtilities;

    internal sealed class LinkIdBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Get input value.
            var name = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(name);

            if (value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(name, value);

            // Deserialize.
            var first = value.FirstValue;
            byte[] raw;

            if (string.IsNullOrEmpty(first))
            {
                return Task.CompletedTask;
            }

            try
            {
                raw = WebEncoders.Base64UrlDecode(first);
            }
            catch (FormatException ex)
            {
                bindingContext.ModelState.TryAddModelError(name, ex, bindingContext.ModelMetadata);
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(new LinkId(raw));

            return Task.CompletedTask;
        }
    }
}
