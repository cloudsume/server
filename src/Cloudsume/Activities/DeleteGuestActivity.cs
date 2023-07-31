namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("93a6ea92-069b-4832-8e32-deaf54ce6f63")]
internal sealed class DeleteGuestActivity : UserActivity
{
    public DeleteGuestActivity(Guid userId, Guid guestId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.GuestId = guestId;
    }

    private DeleteGuestActivity(Guid userId, Ulid id, Guid guestId, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.GuestId = guestId;
    }

    public Guid GuestId { get; }

    public static DeleteGuestActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid guestId;

        try
        {
            guestId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, guestId, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.GuestId));

        return data.Encode();
    }
}
