namespace Cloudsume.DataOperations;

using System;

public sealed class DeleteMultiplicableLocalData : DeleteLocalData
{
    public DeleteMultiplicableLocalData(string key, string type, int index)
        : base(key, type)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        this.Index = index;
    }

    public int Index { get; }

    public void Deconstruct(out string key, out string type, out int index)
    {
        key = this.Key;
        type = this.Type;
        index = this.Index;
    }
}
