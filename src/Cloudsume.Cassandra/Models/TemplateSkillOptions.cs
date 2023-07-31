namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class TemplateSkillOptions
    {
        public static readonly UdtMap Mapping = UdtMap.For<TemplateSkillOptions>("skillopts")
            .Map(o => o.Grouping, "grouping");

        public sbyte Grouping { get; set; }
    }
}
