namespace Candidate.Server.Resume.Builder.Renderers
{
    using System.Globalization;
    using Candidate.Server.Resume.Builder.Values;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Builder;
    using Cloudsume.Resume;
    using Ultima.Extensions.Collections;

    internal sealed class EducationDescriptionRenderer : AttributeRenderer
    {
        private readonly EducationRenderOptions? options;

        public EducationDescriptionRenderer(CultureInfo templateCulture, IKeyedByTypeCollection<TemplateRenderOptions> options)
            : base(templateCulture)
        {
            this.options = options.Get<EducationRenderOptions>();
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

            return renderer.Render(((EducationDescription)obj).Value);
        }
    }
}
