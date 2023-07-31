namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class HeadlineMerger : DataMerger<Headline>
{
    protected override Headline CreateResult(ref LayerSelector<Headline> s)
    {
        var value = s.For(h => h.Value);

        return new(value, s.LastUpdated);
    }
}
