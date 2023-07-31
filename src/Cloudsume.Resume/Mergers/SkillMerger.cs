namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class SkillMerger : DataMerger<Skill>
{
    protected override Skill CreateResult(ref LayerSelector<Skill> s)
    {
        var name = s.For(s => s.Name);
        var level = s.For(s => s.Level);

        return new(null, null, name, level, s.LastUpdated);
    }
}
