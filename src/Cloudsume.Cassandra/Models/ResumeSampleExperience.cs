namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleExperience : MultiplicableSampleData<ResumeSampleExperience, ExperienceData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_experiences");
}
