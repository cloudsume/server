namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class GitHubMerger : DataMerger<GitHub>
{
    protected override GitHub CreateResult(ref LayerSelector<GitHub> s)
    {
        var username = s.For(g => g.Username);

        return new(username, s.LastUpdated);
    }
}
