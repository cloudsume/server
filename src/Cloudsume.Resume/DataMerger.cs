namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using Candidate.Server.Resume;

internal abstract class DataMerger<T> : IDataMerger where T : ResumeData
{
    public Type TargetData => typeof(T);

    public ResumeData Merge(ResumeData top, IParentCollection parents)
    {
        // Create data layers.
        var data = top;
        var layers = new List<T>() { (T)data };

        foreach (var parent in parents)
        {
            GlobalKey key;

            if (data is MultiplicativeData m)
            {
                if (m.BaseId == null)
                {
                    break;
                }

                key = new(m.Type, m.BaseId.Value);
            }
            else
            {
                key = new(data.Type);
            }

            if (!parent.TryGetValue(key, out data))
            {
                break;
            }

            layers.Add((T)data);
        }

        // Merge layers.
        var selector = new LayerSelector<T>(layers);

        return this.CreateResult(ref selector);
    }

    protected abstract T CreateResult(ref LayerSelector<T> s);
}
