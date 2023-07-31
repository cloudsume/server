namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Linq;
using Ultima.Extensions.Collections;

public sealed class TemplateWorkspace
{
    private static readonly IEnumerable<TemplateAsset> RequiredAssets = new TemplateAsset[]
    {
        new(new("main.stg"), 0, DateTime.MinValue),
    };

    public TemplateWorkspace(
        Guid? previewJob,
        IEnumerable<string> applicableData,
        IKeyedByTypeCollection<TemplateRenderOptions> renderOptions,
        IReadOnlySet<TemplateAsset> assets)
    {
        if (applicableData.GroupBy(t => t).Any(g => g.Count() > 1))
        {
            throw new ArgumentException("The value contains duplicated item.", nameof(applicableData));
        }

        this.PreviewJob = previewJob;
        this.ApplicableData = applicableData;
        this.RenderOptions = renderOptions;
        this.Assets = assets;
    }

    public Guid? PreviewJob { get; }

    public IEnumerable<string> ApplicableData { get; }

    public IKeyedByTypeCollection<TemplateRenderOptions> RenderOptions { get; }

    public IReadOnlySet<TemplateAsset> Assets { get; }

    public bool HasRequiredAssets => RequiredAssets.All(this.Assets.Contains);
}
