namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using Cloudsume.Resume;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("d3ff8e09-33b5-4aaa-9e87-9183c9912d73")]
internal sealed class ListLinkAccessActivity : UserActivity
{
    public ListLinkAccessActivity(Guid userId, Guid resumeId, LinkId linkId, Ulid? skipTill, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
        this.SkipTill = skipTill;
    }

    private ListLinkAccessActivity(Guid userId, Ulid id, Guid resumeId, LinkId linkId, Ulid? skipTill, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
        this.SkipTill = skipTill;
    }

    public Guid ResumeId { get; }

    public LinkId LinkId { get; }

    public Ulid? SkipTill { get; }

    public static ListLinkAccessActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid resumeId;
        LinkId linkId;
        Ulid? skipTill;

        try
        {
            resumeId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
            linkId = new(reader.ReadDefiniteLengthByteString().ToArray());

            switch (reader.PeekState())
            {
                case CborReaderState.Null:
                    reader.ReadNull();
                    skipTill = null;
                    break;
                case CborReaderState.ByteString:
                    skipTill = new(reader.ReadDefiniteLengthByteString().Span);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(SkipTill)}.", nameof(data));
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, resumeId, linkId, skipTill, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.ResumeId));
        data.WriteByteString(this.LinkId.Value);

        if (this.SkipTill is { } skipTill)
        {
            data.WriteByteString(skipTill.ToByteArray());
        }
        else
        {
            data.WriteNull();
        }

        return data.Encode();
    }
}
