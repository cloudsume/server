namespace Cloudsume.Activities;

using System;
using System.Collections.Generic;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("4c8b4693-878d-4e61-85d5-27c239678646")]
internal sealed class UpdateResumeActivity : UserActivity
{
    public UpdateResumeActivity(
        Guid userId,
        Guid resumeId,
        IReadOnlySet<(string Type, int? Index)> deletes,
        IReadOnlySet<(string Type, int? Index)> updates,
        int outputPages,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        if (outputPages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(outputPages));
        }

        this.ResumeId = resumeId;
        this.Deletes = deletes;
        this.Updates = updates;
        this.OutputPages = outputPages;
    }

    private UpdateResumeActivity(
        Guid userId,
        Ulid id,
        Guid resumeId,
        IReadOnlySet<(string Type, int? Index)> deletes,
        IReadOnlySet<(string Type, int? Index)> updates,
        int outputPages,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        if (outputPages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(outputPages));
        }

        this.ResumeId = resumeId;
        this.Deletes = deletes;
        this.Updates = updates;
        this.OutputPages = outputPages;
    }

    public Guid ResumeId { get; }

    public IReadOnlySet<(string Type, int? Index)> Deletes { get; }

    public IReadOnlySet<(string Type, int? Index)> Updates { get; }

    public int OutputPages { get; }

    public static UpdateResumeActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        var deletes = new HashSet<(string Type, int? Index)>();
        var updates = new HashSet<(string Type, int? Index)>();
        Guid resumeId;
        int outputPages;

        try
        {
            resumeId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);

            // Deletes.
            if (reader.ReadStartArray() is not { } deleteCount)
            {
                throw new ArgumentException($"Invalid {nameof(Deletes)}.", nameof(data));
            }

            for (var i = 0; i < deleteCount; i++)
            {
                if (reader.ReadStartArray() != 2)
                {
                    throw new ArgumentException($"Invalid {nameof(Deletes)}.", nameof(data));
                }

                var type = Encoding.UTF8.GetString(reader.ReadDefiniteLengthTextStringBytes().Span);
                int? index;

                switch (reader.PeekState())
                {
                    case CborReaderState.Null:
                        reader.ReadNull();
                        index = null;
                        break;
                    case CborReaderState.UnsignedInteger:
                        index = reader.ReadInt32();
                        break;
                    default:
                        throw new ArgumentException($"Invalid {nameof(Deletes)}.", nameof(data));
                }

                reader.ReadEndArray();

                if (!deletes.Add((type, index)))
                {
                    throw new ArgumentException($"{nameof(Deletes)} has a duplicated item.", nameof(data));
                }
            }

            reader.ReadEndArray();

            // Updates.
            if (reader.ReadStartArray() is not { } updateCount)
            {
                throw new ArgumentException($"Invalid {nameof(Updates)}.", nameof(data));
            }

            for (var i = 0; i < updateCount; i++)
            {
                if (reader.ReadStartArray() != 2)
                {
                    throw new ArgumentException($"Invalid {nameof(Updates)}.", nameof(data));
                }

                var type = Encoding.UTF8.GetString(reader.ReadDefiniteLengthTextStringBytes().Span);
                int? index;

                switch (reader.PeekState())
                {
                    case CborReaderState.Null:
                        reader.ReadNull();
                        index = null;
                        break;
                    case CborReaderState.UnsignedInteger:
                        index = reader.ReadInt32();
                        break;
                    default:
                        throw new ArgumentException($"Invalid {nameof(Updates)}.", nameof(data));
                }

                reader.ReadEndArray();

                if (!updates.Add((type, index)))
                {
                    throw new ArgumentException($"{nameof(Updates)} has a duplicated item.", nameof(data));
                }
            }

            reader.ReadEndArray();

            // OutputPages.
            outputPages = reader.ReadInt32();
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException || ex is OverflowException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, resumeId, deletes, updates, outputPages, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.ResumeId));
        data.WriteStartArray(this.Deletes.Count);

        foreach (var delete in this.Deletes)
        {
            data.WriteStartArray(2);
            data.WriteTextString(delete.Type);

            if (delete.Index is { } index)
            {
                data.WriteInt32(index);
            }
            else
            {
                data.WriteNull();
            }

            data.WriteEndArray();
        }

        data.WriteEndArray();
        data.WriteStartArray(this.Updates.Count);

        foreach (var update in this.Updates)
        {
            data.WriteStartArray(2);
            data.WriteTextString(update.Type);

            if (update.Index is { } index)
            {
                data.WriteInt32(index);
            }
            else
            {
                data.WriteNull();
            }

            data.WriteEndArray();
        }

        data.WriteEndArray();
        data.WriteInt32(this.OutputPages);

        return data.Encode();
    }
}
