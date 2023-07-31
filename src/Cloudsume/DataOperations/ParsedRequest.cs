namespace Cloudsume.DataOperations;

using System;
using System.Collections.Generic;

internal sealed class ParsedRequest<TDeletionId>
    where TDeletionId : struct, IComparable<TDeletionId>
{
    public ParsedRequest(
        IEnumerable<KeyValuePair<UpdateKey, object>> updates,
        IReadOnlySet<DeleteKey<TDeletionId>> deletes,
        IReadOnlyDictionary<string, object> contents)
    {
        this.Updates = updates;
        this.Deletes = deletes;
        this.Contents = contents;
    }

    public IEnumerable<KeyValuePair<UpdateKey, object>> Updates { get; }

    public IReadOnlySet<DeleteKey<TDeletionId>> Deletes { get; }

    public IReadOnlyDictionary<string, object> Contents { get; }
}
