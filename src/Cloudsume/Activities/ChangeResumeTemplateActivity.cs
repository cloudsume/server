namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Primitives;

[Guid("2bbd7ac3-ff99-44e5-b820-90a1999e07d8")]
internal sealed class ChangeResumeTemplateActivity : UserActivity
{
    public ChangeResumeTemplateActivity(Guid userId, Guid resumeId, Ulid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.TemplateId = templateId;
    }

    private ChangeResumeTemplateActivity(Guid userId, Ulid id, Guid resumeId, Ulid templateId, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        this.ResumeId = resumeId;
        this.TemplateId = templateId;
    }

    public Guid ResumeId { get; }

    public Ulid TemplateId { get; }

    public static ChangeResumeTemplateActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid resumeId;
        Ulid templateId;

        try
        {
            resumeId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);
            templateId = new(reader.ReadDefiniteLengthByteString().Span);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, resumeId, templateId, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.ResumeId));
        data.WriteByteString(this.TemplateId.ToByteArray());

        return data.Encode();
    }
}
