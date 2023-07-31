namespace Cloudsume.DataOperations;

using System;

public sealed class UpdateMultiplicableLocalData : UpdateLocalData
{
    public UpdateMultiplicableLocalData(string key, Candidate.Server.Resume.MultiplicativeData update, int index)
        : base(key, update)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        this.Index = index;
    }

    public new Candidate.Server.Resume.MultiplicativeData Update => (Candidate.Server.Resume.MultiplicativeData)base.Update;

    public int Index { get; }

    public void Deconstruct(out string key, out Candidate.Server.Resume.MultiplicativeData update, out int index)
    {
        key = this.Key;
        update = this.Update;
        index = this.Index;
    }
}
