namespace Cloudsume.DataOperations;

using System;

internal sealed class UpdateKey : FormKey, IComparable<UpdateKey>, IComparable
{
    public UpdateKey(string type, string? raw)
        : base(raw)
    {
        this.Type = type;
    }

    public UpdateKey(string type, int index, string? raw)
        : base(raw)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        this.Type = type;
        this.Index = index;
    }

    public string Type { get; }

    public int? Index { get; }

    public int CompareTo(UpdateKey? other)
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

        // Compare index.
        if (this.Index is null && other.Index is null)
        {
            return 0;
        }
        else if (this.Index is null)
        {
            return -1;
        }
        else if (other.Index is null)
        {
            return 1;
        }
        else
        {
            return this.Index.Value - other.Index.Value;
        }
    }

    public int CompareTo(object? obj)
    {
        if (obj is not null && obj.GetType() != this.GetType())
        {
            throw new ArgumentException($"The value is not an instance of {this.GetType()}.", nameof(obj));
        }

        return this.CompareTo((UpdateKey?)obj);
    }
}
