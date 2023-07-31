namespace Cloudsume.DataOperations;

using System;

internal sealed class DeleteKey<TId> : FormKey, IComparable<DeleteKey<TId>>, IComparable
    where TId : struct, IComparable<TId>
{
    public DeleteKey(string type, string? raw)
        : base(raw)
    {
        this.Type = type;
    }

    public DeleteKey(string type, TId id, string? raw)
        : base(raw)
    {
        this.Type = type;
        this.Id = id;
    }

    public string Type { get; }

    public TId? Id { get; }

    public int CompareTo(DeleteKey<TId>? other)
    {
        if (other is null)
        {
            return 1;
        }

        // Compare type.
        switch (string.CompareOrdinal(this.Type, other.Type))
        {
            case > 0:
                return 1;
            case < 0:
                return -1;
        }

        // Compare ID.
        if (this.Id is null && other.Id is null)
        {
            return 0;
        }
        else if (this.Id is null)
        {
            return -1;
        }
        else if (other.Id is null)
        {
            return 1;
        }
        else
        {
            return this.Id.Value.CompareTo(other.Id.Value);
        }
    }

    public int CompareTo(object? obj)
    {
        if (obj is not null && obj.GetType() != this.GetType())
        {
            throw new ArgumentException($"The value is not an instance of {this.GetType()}.", nameof(obj));
        }

        return this.CompareTo((DeleteKey<TId>?)obj);
    }
}
