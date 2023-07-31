namespace Cloudsume.Cassandra
{
    using Candidate.Server.Resume.Data;
    using Cloudsume.Resume;
    using Ultima.Extensions.Collections;

    internal static class IRenderOptionsExtensions
    {
        public static IKeyedByTypeCollection<TemplateRenderOptions> GetRenderOptions(this Models.IRenderOptions row)
        {
            var domain = new KeyedByTypeCollection<TemplateRenderOptions>();

            if (row.ExperienceOptions is { } experience)
            {
                domain.Add(new ExperienceRenderOptions(experience.DescriptionParagraph, experience.DescriptionListOptions));
            }

            if (row.EducationOptions is { } education)
            {
                domain.Add(new EducationRenderOptions(education.DescriptionParagraph, education.DescriptionListOptions));
            }

            if (row.SkillOptions is { } skill)
            {
                domain.Add(new SkillRenderOptions(skill.Grouping is { } g ? (SkillRenderOptions.GroupingType)g : SkillRenderOptions.GroupingType.None));
            }

            return domain;
        }
    }
}
