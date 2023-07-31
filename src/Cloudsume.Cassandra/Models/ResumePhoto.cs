namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumePhoto : ResumeData<ResumePhoto, PhotoData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_photos");
    }
}
