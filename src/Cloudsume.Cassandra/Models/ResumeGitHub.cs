namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra.Mapping;

    public sealed class ResumeGitHub : ResumeData<ResumeGitHub, GitHubData>
    {
        public static readonly ITypeDefinition Mapping = CreateMapping("resume_githubs");
    }
}
