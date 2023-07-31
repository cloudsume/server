namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleGitHub : ResumeSampleData<ResumeSampleGitHub, GitHubData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_githubs");
}
