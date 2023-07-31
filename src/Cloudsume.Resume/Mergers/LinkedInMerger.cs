namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class LinkedInMerger : DataMerger<LinkedIn>
{
    protected override LinkedIn CreateResult(ref LayerSelector<LinkedIn> s)
    {
        var username = s.For(l => l.Username);

        return new(username, s.LastUpdated);
    }
}
