namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class SummaryMerger : DataMerger<Summary>
{
    protected override Summary CreateResult(ref LayerSelector<Summary> s)
    {
        var value = s.For(s => s.Value);

        return new(value, s.LastUpdated);
    }
}
