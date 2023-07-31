namespace Cloudsume.Activities;

using System;
using System.Collections.Generic;
using System.Formats.Cbor;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("d542a151-4605-43d8-a48b-a3be6055fd74")]
internal sealed class UpdateDataActivity : UserActivity
{
    public UpdateDataActivity(
        Guid userId,
        CultureInfo culture,
        IReadOnlySet<(string Type, Guid? Id)> updates,
        IReadOnlySet<Guid> affectedResumes,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.Culture = culture;
        this.Updates = updates;
        this.AffectedResumes = affectedResumes;
    }

    private UpdateDataActivity(
        Guid userId,
        Ulid id,
        CultureInfo culture,
        IReadOnlySet<(string Type, Guid? Id)> updates,
        IReadOnlySet<Guid> affectedResumes,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.Culture = culture;
        this.Updates = updates;
        this.AffectedResumes = affectedResumes;
    }

    public CultureInfo Culture { get; }

    public IReadOnlySet<(string Type, Guid? Id)> Updates { get; }

    public IReadOnlySet<Guid> AffectedResumes { get; }

    public static UpdateDataActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        var updates = new HashSet<(string Type, Guid? Id)>();
        var affectedResumes = new HashSet<Guid>();
        CultureInfo culture;

        try
        {
            // Culture.
            try
            {
                culture = CultureInfo.GetCultureInfo(Encoding.UTF8.GetString(reader.ReadDefiniteLengthTextStringBytes().Span));
            }
            catch (CultureNotFoundException ex)
            {
                throw new ArgumentException($"Invalid {nameof(Culture)}.", nameof(data), ex);
            }

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
                Guid? dataId;

                switch (reader.PeekState())
                {
                    case CborReaderState.Null:
                        reader.ReadNull();
                        dataId = null;
                        break;
                    case CborReaderState.ByteString:
                        dataId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
                        break;
                    default:
                        throw new ArgumentException($"Invalid {nameof(Updates)}.", nameof(data));
                }

                reader.ReadEndArray();

                if (!updates.Add((type, dataId)))
                {
                    throw new ArgumentException($"{nameof(Updates)} has a duplicated item.", nameof(data));
                }
            }

            reader.ReadEndArray();

            // AffectedResumes
            if (reader.ReadStartArray() is not { } affectedResumeCount)
            {
                throw new ArgumentException($"Invalid {nameof(AffectedResumes)}.", nameof(data));
            }

            for (var i = 0; i < affectedResumeCount; i++)
            {
                if (!affectedResumes.Add(Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span)))
                {
                    throw new ArgumentException($"{nameof(AffectedResumes)} has a duplicated item.", nameof(data));
                }
            }

            reader.ReadEndArray();
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, culture, updates, affectedResumes, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteTextString(this.Culture.Name);
        data.WriteStartArray(this.Updates.Count);

        foreach (var update in this.Updates)
        {
            data.WriteStartArray(2);
            data.WriteTextString(update.Type);

            if (update.Id is { } id)
            {
                data.WriteByteString(Uuid.FromGuid(id));
            }
            else
            {
                data.WriteNull();
            }

            data.WriteEndArray();
        }

        data.WriteEndArray();
        data.WriteStartArray(this.AffectedResumes.Count);

        foreach (var id in this.AffectedResumes)
        {
            data.WriteByteString(Uuid.FromGuid(id));
        }

        data.WriteEndArray();

        return data.Encode();
    }
}
