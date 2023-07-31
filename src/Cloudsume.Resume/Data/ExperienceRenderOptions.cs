namespace Candidate.Server.Resume.Data
{
    using System;
    using Cloudsume.Resume;

    public sealed class ExperienceRenderOptions : TemplateRenderOptions
    {
        public ExperienceRenderOptions(string? descriptionParagraph, string? descriptionListOptions)
        {
            this.DescriptionParagraph = descriptionParagraph;
            this.DescriptionListOptions = descriptionListOptions;
        }

        public override Type TargetData => typeof(Experience);

        public string? DescriptionParagraph { get; }

        public string? DescriptionListOptions { get; }
    }
}
