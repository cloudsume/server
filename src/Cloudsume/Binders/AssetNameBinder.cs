namespace Cloudsume.Binders;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AssetName = Cloudsume.Resume.AssetName;

internal sealed class AssetNameBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Get input value.
        var model = bindingContext.ModelName;
        var value = bindingContext.ValueProvider.GetValue(model);

        if (value == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(model, value);

        // Deserialize.
        var first = value.FirstValue;
        AssetName result;

        if (string.IsNullOrEmpty(first))
        {
            return Task.CompletedTask;
        }
        else if (first == "main.tex" || first.StartsWith("main.tex/", StringComparison.Ordinal))
        {
            bindingContext.ModelState.TryAddModelError(model, "The value is not a valid asset name.");
            return Task.CompletedTask;
        }

        try
        {
            result = new(first);
        }
        catch (ArgumentException ex)
        {
            bindingContext.ModelState.TryAddModelError(model, ex, bindingContext.ModelMetadata);
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(result);

        return Task.CompletedTask;
    }
}
