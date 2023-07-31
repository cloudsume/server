namespace Candidate.Server.Resume.Builder
{
    using System.Globalization;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Builder;
    using Cloudsume.Resume;
    using Ultima.Extensions.Collections;

    internal sealed class ExperienceDescriptionRenderer : AttributeRenderer
    {
        private readonly ExperienceRenderOptions? options;

        public ExperienceDescriptionRenderer(CultureInfo templateCulture, IKeyedByTypeCollection<TemplateRenderOptions> options)
            : base(templateCulture)
        {
            this.options = options.Get<ExperienceRenderOptions>();
        }

        public override string Render(object obj, string format, CultureInfo culture)
        {
            var renderer = this.CreateMarkdownRenderer();

            if (this.options is { } options)
            {
                if (options.DescriptionParagraph is { } newParagraph)
                {
                    renderer.NewParagraph = newParagraph;
                }

                if (options.DescriptionListOptions is { } listOptions)
                {
                    renderer.ListOptions = listOptions;
                }
            }

            return renderer.Render(((ExperienceDescription)obj).Value);
        }
    }
}
