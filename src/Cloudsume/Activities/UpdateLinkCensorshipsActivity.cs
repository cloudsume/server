namespace Cloudsume.Activities;

using System;
using System.Collections.Generic;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Cloudsume.Analytics;
using Cloudsume.Resume;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("fdfbaa6a-f194-468b-aba4-af65e40bd73d")]
internal sealed class UpdateLinkCensorshipsActivity : UserActivity
{
    public UpdateLinkCensorshipsActivity(
        Guid userId,
        Guid resumeId,
        LinkId linkId,
        IReadOnlySet<LinkCensorship> censorships,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
        this.Censorships = censorships;
    }

    private UpdateLinkCensorshipsActivity(
        Guid userId,
        Ulid id,
        Guid resumeId,
        LinkId linkId,
        IReadOnlySet<LinkCensorship> censorships,
        IPAddress ipAddress,
        string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.LinkId = linkId;
        this.Censorships = censorships;
    }

    public Guid ResumeId { get; }

    public LinkId LinkId { get; }

    public IReadOnlySet<LinkCensorship> Censorships { get; }

    public static UpdateLinkCensorshipsActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        var censorships = new HashSet<LinkCensorship>();
        Guid resumeId;
        LinkId linkId;

        try
        {
            resumeId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
            linkId = new(reader.ReadDefiniteLengthByteString().ToArray());

            if (reader.ReadStartArray() is not { } count)
            {
                throw new ArgumentException($"Invalid {nameof(Censorships)}.", nameof(data));
            }

            for (var i = 0; i < count; i++)
            {
                LinkCensorship censorship;

                try
                {
                    censorship = LinkCensorship.Parse(Encoding.UTF8.GetString(reader.ReadDefiniteLengthTextStringBytes().Span));
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException($"Invalid {nameof(Censorships)}.", nameof(data), ex);
                }

                if (!censorships.Add(censorship))
                {
                    throw new ArgumentException($"{nameof(Censorships)} has a duplicated item.", nameof(data));
                }
            }

            reader.ReadEndArray();
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, resumeId, linkId, censorships, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.ResumeId));
        data.WriteByteString(this.LinkId.Value);
        data.WriteStartArray(this.Censorships.Count);

        foreach (var censorship in this.Censorships)
        {
            data.WriteTextString(censorship.ToString());
        }

        data.WriteEndArray();

        return data.Encode();
    }
}
