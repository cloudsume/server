namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    public sealed class EducationRenderOptions : TemplateRenderOptions
    {
        public EducationRenderOptions(string? descriptionParagraph, string? descriptionListOptions)
        {
            this.DescriptionParagraph = descriptionParagraph;
            this.DescriptionListOptions = descriptionListOptions;
        }

        public override Type TargetData => typeof(Education);

        public string? DescriptionParagraph { get; }

        public string? DescriptionListOptions { get; }
    }
}
