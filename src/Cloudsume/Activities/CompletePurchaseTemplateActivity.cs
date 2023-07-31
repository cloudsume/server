namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("3f677584-a8e5-48ba-ac8d-350587e46e05")]
internal sealed class CompletePurchaseTemplateActivity : UserActivity
{
    public CompletePurchaseTemplateActivity(Guid userId, Guid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.TemplateId = templateId;
    }

    private CompletePurchaseTemplateActivity(Guid userId, Ulid id, Guid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.TemplateId = templateId;
    }

    public Guid TemplateId { get; }

    public static CompletePurchaseTemplateActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid templateId;

        try
        {
            templateId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, templateId, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.TemplateId));

        return data.Encode();
    }
}
