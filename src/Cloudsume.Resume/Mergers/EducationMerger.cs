namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class EducationMerger : DataMerger<Education>
{
    protected override Education CreateResult(ref LayerSelector<Education> s)
    {
        var institute = s.For(e => e.Institute);
        var region = s.For(e => e.Region);
        var degreeName = s.For(e => e.DegreeName);
        var start = s.For(e => e.Start);
        var end = s.For(e => e.End);
        var grade = s.For(e => e.Grade);
        var description = s.For(e => e.Description);

        return new(null, null, institute, region, degreeName, start, end, grade, description, s.LastUpdated);
    }
}
