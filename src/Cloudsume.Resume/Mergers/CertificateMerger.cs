namespace Cloudsume.Resume.Mergers;

using Cloudsume.Resume.Data;

internal sealed class CertificateMerger : DataMerger<Certificate>
{
    protected override Certificate CreateResult(ref LayerSelector<Certificate> s)
    {
        var name = s.For(c => c.Name);
        var obtained = s.For(c => c.Obtained);

        return new(null, null, name, obtained, s.LastUpdated);
    }
}
