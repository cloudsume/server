namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class ExperienceMerger : DataMerger<Experience>
{
    protected override Experience CreateResult(ref LayerSelector<Experience> s)
    {
        var start = s.For(e => e.Start);
        var end = s.For(e => e.End);
        var title = s.For(e => e.Title);
        var company = s.For(e => e.Company);
        var region = s.For(e => e.Region);
        var street = s.For(e => e.Street);
        var description = s.For(e => e.Description);

        return new(null, null, start, end, title, company, region, street, description, s.LastUpdated);
    }
}
