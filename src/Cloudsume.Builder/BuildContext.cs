namespace Cloudsume.Builder;

using System.Globalization;
using Cloudsume.Resume;
using Cloudsume.Services.Client;
using Ultima.Extensions.Collections;

public sealed class BuildContext
{
    public BuildContext(CultureInfo culture, ILatexJobPoster job, IKeyedByTypeCollection<TemplateRenderOptions> options)
    {
        this.Culture = culture;
        this.Job = job;
        this.Options = options;
    }

    public CultureInfo Culture { get; }

    public ILatexJobPoster Job { get; }

    public IKeyedByTypeCollection<TemplateRenderOptions> Options { get; }
}
