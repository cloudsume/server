namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleLanguage : MultiplicableSampleData<ResumeSampleLanguage, LanguageData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_languages");
}
