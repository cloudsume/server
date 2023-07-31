namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;
using Cloudsume.Resume.DataSources;

internal sealed class PhotoMerger : DataMerger<Photo>
{
    protected override Photo CreateResult(ref LayerSelector<Photo> s)
    {
        var layer = s.Select(p => p.Info);

        if (layer is null)
        {
            return new(new(null, new FromAggregator()), s.LastUpdated);
        }

        return new(layer.Info, s.LastUpdated)
        {
            Image = layer.Image,
        };
    }
}
