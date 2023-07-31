namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class EmailAddressMerger : DataMerger<EmailAddress>
{
    protected override EmailAddress CreateResult(ref LayerSelector<EmailAddress> s)
    {
        var value = s.For(e => e.Value);

        return new(value, s.LastUpdated);
    }
}
