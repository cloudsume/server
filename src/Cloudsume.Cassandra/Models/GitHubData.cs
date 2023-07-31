namespace Cloudsume.Cassandra.Models;

using global::Cassandra;

[DomainType(typeof(Candidate.Server.Resume.Data.GitHub))]
public sealed class GitHubData : DataObject
{
    public static readonly UdtMap Mapping = CreateMapping<GitHubData>("resume_github")
        .Map(d => d.Username, "username");

    public TextProperty? Username { get; set; }
}
