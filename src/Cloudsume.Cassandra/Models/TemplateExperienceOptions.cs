namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class TemplateExperienceOptions
    {
        public static readonly UdtMap Mapping = UdtMap.For<TemplateExperienceOptions>("expopts")
            .Map(o => o.DescriptionParagraph, "desc_paragraph")
            .Map(o => o.DescriptionListOptions, "desc_list_options");

        public string? DescriptionParagraph { get; set; }

        public string? DescriptionListOptions { get; set; }
    }
}
