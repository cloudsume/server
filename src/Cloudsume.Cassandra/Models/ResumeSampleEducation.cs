namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleEducation : MultiplicableSampleData<ResumeSampleEducation, EducationData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_educations");
}
