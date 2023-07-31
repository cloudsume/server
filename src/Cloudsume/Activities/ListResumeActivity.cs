namespace Cloudsume.Activities;

using System;
using System.Net;
using System.Runtime.InteropServices;
using Cloudsume.Analytics;
using NetUlid;

[Guid("65185254-34f2-40a1-997c-d78eafe407fd")]
internal sealed class ListResumeActivity : UserActivity
{
    public ListResumeActivity(Guid userId, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
    }

    private ListResumeActivity(Guid userId, Ulid id, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
    }

    public static ListResumeActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        return new(userId, id, ipAddress, userAgent);
    }

    public override byte[] Serialize() => Array.Empty<byte>();
}
