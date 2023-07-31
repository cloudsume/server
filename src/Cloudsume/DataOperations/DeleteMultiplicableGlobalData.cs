namespace Cloudsume.DataOperations;

using System;

public sealed class DeleteMultiplicableGlobalData : DeleteGlobalData
{
    public DeleteMultiplicableGlobalData(string key, string type, Guid id)
        : base(key, type)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("The value is an empty GUID.", nameof(id));
        }

        this.Id = id;
    }

    public Guid Id { get; }
}
