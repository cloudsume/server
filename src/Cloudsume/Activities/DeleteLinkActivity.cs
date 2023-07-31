namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using Cloudsume.Resume;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("c761b08b-2cd7-41e5-a946-42380ba467bd")]
internal sealed class DeleteLinkActivity : UserActivity
{
    public DeleteLinkActivity(Guid userId, Guid resumeId, LinkId linkId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
    }

    private DeleteLinkActivity(Guid userId, Ulid id, Guid resumeId, LinkId linkId, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
    }

    public Guid ResumeId { get; }

    public LinkId LinkId { get; }

    public static DeleteLinkActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid resumeId;
        LinkId linkId;

        try
        {
            resumeId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
            linkId = new(reader.ReadDefiniteLengthByteString().ToArray());
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, resumeId, linkId, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.ResumeId));
        data.WriteByteString(this.LinkId.Value);

        return data.Encode();
    }
}
