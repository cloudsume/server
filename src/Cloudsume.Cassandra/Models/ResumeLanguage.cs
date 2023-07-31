namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeLanguage : MultiplicableResumeData<ResumeLanguage, LanguageData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_languages");
    }
}
