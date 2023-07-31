namespace Cloudsume.Resume;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Layer = System.Collections.Generic.IReadOnlyDictionary<Cloudsume.Resume.GlobalKey, Candidate.Server.Resume.ResumeData>;

public sealed class ParentCollection : IParentCollection
{
    private readonly List<Layer> layers;

    public ParentCollection()
    {
        this.layers = new();
    }

    private ParentCollection(List<Layer> layers)
    {
        this.layers = layers;
    }

    public int Count => this.layers.Count;

    public Layer this[int index] => this.layers[index];

    public static async Task<IParentCollection> LoadAsync(CultureInfo start, Func<CultureInfo, Task<IEnumerable<ResumeData>>> loader)
    {
        var layers = new List<Layer>();

        for (var culture = start; ;)
        {
            // Populate a list for current culture.
            var layer = new Dictionary<GlobalKey, ResumeData>();

            foreach (var data in await loader(culture))
            {
                layer.Add(new(data), data);
            }

            layers.Add(layer);

            // Move to lower culture.
            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                break;
            }
            else
            {
                culture = culture.Parent;
            }
        }

        return new ParentCollection(layers);
    }

    public void Add(Layer layer)
    {
        this.layers.Add(layer);
    }

    public IEnumerator<Layer> GetEnumerator() => this.layers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
