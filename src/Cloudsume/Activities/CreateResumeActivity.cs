namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using NetUlid;

[Guid("97b39713-a8ab-4ba3-9752-0a3fbde41843")]
internal sealed class CreateResumeActivity : UserActivity
{
    public CreateResumeActivity(Guid userId, Ulid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.TemplateId = templateId;
    }

    private CreateResumeActivity(Guid userId, Ulid id, Ulid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.TemplateId = templateId;
    }

    public Ulid TemplateId { get; }

    public static CreateResumeActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Ulid templateId;

        try
        {
            templateId = new(reader.ReadDefiniteLengthByteString().Span);
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

        data.WriteByteString(this.TemplateId.ToByteArray());

        return data.Encode();
    }
}
