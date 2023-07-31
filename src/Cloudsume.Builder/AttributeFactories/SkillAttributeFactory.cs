namespace Cloudsume.Builder.AttributeFactories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume.Data;
using Cornot;

internal sealed class SkillAttributeFactory : ListAttributeFactory<Skill>
{
    public override async ValueTask<object?> CreateAsync(BuildContext context, IEnumerable<Skill> data, CancellationToken cancellationToken)
    {
        var options = context.Options.Get<SkillRenderOptions>();

        return options?.Grouping switch
        {
            SkillRenderOptions.GroupingType.Level => this.CreateLeveledGroups(data),
            _ => await base.CreateAsync(context, data, cancellationToken),
        };
    }

    protected override object? Create(BuildContext context, Skill data) => new
    {
        Name = TexString.From(data.Name.Value),
    };

    private object? CreateLeveledGroups(IEnumerable<Skill> data)
    {
        var result = new Dictionary<string, IEnumerable<TexString?>>();

        foreach (var g in data.GroupBy(d => d.Level.Value ?? SkillLevel.Expert))
        {
            var names = new List<TexString?>();
            var key = g.Key switch
            {
                SkillLevel.Novice => "novice",
                SkillLevel.Expert => "expert",
                _ => throw new DataCorruptionException(g.First(), "Unknow level."),
            };

            foreach (var d in g)
            {
                names.Add(TexString.From(d.Name.Value));
            }

            result.Add(key, names);
        }

        return result;
    }
}
