namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class MobileMerger : DataMerger<Mobile>
{
    protected override Mobile CreateResult(ref LayerSelector<Mobile> s)
    {
        var value = s.For(m => m.Value);

        return new(value, s.LastUpdated);
    }
}
