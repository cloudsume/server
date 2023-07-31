namespace Cloudsume.Cassandra
{
    using System;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Resume;
    using Ultima.Extensions.Collections;

    internal static class TemplateRenderOptionsExtensions
    {
        public static Models.TemplateEducationOptions? GetEducationOptions(this IKeyedByTypeCollection<TemplateRenderOptions> options)
        {
            var option = options.Get<EducationRenderOptions>();

            if (option == null)
            {
                return null;
            }

            return new()
            {
                DescriptionParagraph = option.DescriptionParagraph,
                DescriptionListOptions = option.DescriptionListOptions,
            };
        }

        public static Models.TemplateExperienceOptions? GetExperienceOptions(this IKeyedByTypeCollection<TemplateRenderOptions> options)
        {
            var option = options.Get<ExperienceRenderOptions>();

            if (option == null)
            {
                return null;
            }

            return new()
            {
                DescriptionParagraph = option.DescriptionParagraph,
                DescriptionListOptions = option.DescriptionListOptions,
            };
        }

        public static Models.TemplateSkillOptions? GetSkillOptions(this IKeyedByTypeCollection<TemplateRenderOptions> options)
        {
            var option = options.Get<SkillRenderOptions>();

            if (option == null)
            {
                return null;
            }

            return new()
            {
                Grouping = Convert.ToSByte(option.Grouping),
            };
        }
    }
}
