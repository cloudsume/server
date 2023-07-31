namespace Cloudsume.Cassandra.Models
{
    public interface IRenderOptions
    {
        TemplateExperienceOptions? ExperienceOptions { get; set; }

        TemplateEducationOptions? EducationOptions { get; set; }

        TemplateSkillOptions? SkillOptions { get; set; }
    }
}
