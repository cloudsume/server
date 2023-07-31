namespace Cloudsume.Resume.Mergers;

using Cloudsume.Resume.Data;

internal sealed class AddressMerger : DataMerger<Address>
{
    protected override Address CreateResult(ref LayerSelector<Address> s)
    {
        var region = s.For(a => a.Region);
        var street = s.For(a => a.Street);

        return new(region, street, s.LastUpdated);
    }
}
