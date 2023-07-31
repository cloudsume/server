namespace Cloudsume.Resume.Mergers;

using Cloudsume.Resume.Data;

internal sealed class WebsiteMerger : DataMerger<Website>
{
    protected override Website CreateResult(ref LayerSelector<Website> s)
    {
        var value = s.For(w => w.Value);

        return new(value, s.LastUpdated);
    }
}
