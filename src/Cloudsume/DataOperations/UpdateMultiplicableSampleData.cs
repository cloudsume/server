namespace Cloudsume.DataOperations;

using System;

public sealed class UpdateMultiplicableSampleData : UpdateSampleData
{
    public UpdateMultiplicableSampleData(string key, Cloudsume.Resume.SampleData update, int index)
        : base(key, update)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        this.Index = index;
    }

    public int Index { get; }

    public void Deconstruct(out string key, out Cloudsume.Resume.SampleData update, out int index)
    {
        key = this.Key;
        update = this.Update;
        index = this.Index;
    }
}
