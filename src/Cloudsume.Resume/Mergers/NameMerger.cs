namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class NameMerger : DataMerger<Name>
{
    protected override Name CreateResult(ref LayerSelector<Name> s)
    {
        var firstName = s.For(n => n.FirstName);
        var middleName = s.For(n => n.MiddleName);
        var lastName = s.For(n => n.LastName);

        return new(firstName, middleName, lastName, s.LastUpdated);
    }
}
