namespace Cloudsume.DataOperations;

using System;
using System.Globalization;

internal abstract class FormKey
{
    private readonly string? raw;

    protected FormKey(string? raw)
    {
        this.raw = raw;
    }

    public static implicit operator string(FormKey key) => key.ToString();

    public static FormKey Parse<TDeletionId>(string raw, Func<string, TDeletionId> deletionId) where TDeletionId : struct, IComparable<TDeletionId>
    {
        // Get the index of ':'.
        var sep = raw.IndexOf(':');

        if (sep == -1)
        {
            throw new ArgumentException("The value is not a valid name for data operation.", nameof(raw));
        }

        // Extract operation type.
        var operation = raw[..sep];
        var remainder = raw[(sep + 1)..];

        // Parse the operation.
        return operation switch
        {
            "update" => ParseUpdate(remainder),
            "delete" => ParseDelete(remainder),
            "content" => new ContentKey(remainder, raw),
            _ => throw new ArgumentException("Invalid operation type.", nameof(raw)),
        };

        UpdateKey ParseUpdate(string s)
        {
            // Get data type.
            var sep = s.IndexOf(':');

            if (sep == -1)
            {
                return new(s, raw);
            }

            var type = s[..sep];
            var remainder = s[(sep + 1)..];

            // Get index.
            int index;

            try
            {
                index = int.Parse(remainder, CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                throw new ArgumentException("Invalid index.", nameof(s), ex);
            }

            if (index < 0)
            {
                throw new ArgumentException("Invalid index.", nameof(s));
            }

            return new(type, index, raw);
        }

        DeleteKey<TDeletionId> ParseDelete(string s)
        {
            // Get data type.
            var sep = s.IndexOf(':');

            if (sep == -1)
            {
                return new(s, raw);
            }

            var type = s[..sep];
            var remainder = s[(sep + 1)..];

            // Get target ID.
            TDeletionId id;

            try
            {
                id = deletionId.Invoke(remainder);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                throw new ArgumentException("Invalid identifier.", nameof(s), ex);
            }

            return new(type, id, raw);
        }
    }

    public override string ToString() => this.raw ?? string.Empty;
}
