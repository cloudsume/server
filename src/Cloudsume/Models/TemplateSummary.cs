namespace Cloudsume.Models;

using Domain = Cloudsume.Resume.Template;

public sealed class TemplateSummary : TemplateInfo
{
    public TemplateSummary(Domain domain, string preview)
        : base(domain)
    {
        this.Preview = preview;
    }

    public string Preview { get; }
}
