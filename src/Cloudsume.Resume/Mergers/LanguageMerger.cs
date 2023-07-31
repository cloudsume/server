namespace Cloudsume.Resume.Mergers;

using Candidate.Server.Resume.Data;

internal sealed class LanguageMerger : DataMerger<Language>
{
    protected override Language CreateResult(ref LayerSelector<Language> s)
    {
        var language = s.For(l => l.Value);
        var proficiency = s.For(l => l.Proficiency);
        var comment = s.For(l => l.Comment);

        return new(null, null, language, proficiency, comment, s.LastUpdated);
    }
}
