namespace Cloudsume.Resume;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

internal sealed class DataActionCollection<T> : IDataActionCollection<T> where T : IDataAction
{
    private readonly Dictionary<string, T> actions;

    public DataActionCollection(IEnumerable<T> actions)
    {
        this.actions = new();

        foreach (var action in actions)
        {
            var model = action.TargetData;
            var attribute = (ResumeDataAttribute)model.GetCustomAttributes(typeof(ResumeDataAttribute), false).Single();

            this.actions.Add(attribute.Type, action);
        }
    }

    public int Count => this.actions.Count;

    public IEnumerable<string> Keys => this.actions.Keys;

    public IEnumerable<T> Values => this.actions.Values;

    public T this[string key] => this.actions[key];

    public bool ContainsKey(string key)
    {
        return this.actions.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return this.actions.GetEnumerator();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value)
    {
        return this.actions.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
