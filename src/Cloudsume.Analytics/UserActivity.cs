namespace Cloudsume.Analytics;

using System;
using System.Net;
using NetUlid;

public abstract class UserActivity
{
    protected UserActivity(Guid userId, IPAddress ipAddress, string userAgent)
        : this(userId, Ulid.Generate(), ipAddress, userAgent)
    {
    }

    protected UserActivity(Guid userId, Ulid id, IPAddress ipAddress, string userAgent)
    {
        this.UserId = userId;
        this.Id = id;
        this.IpAddress = ipAddress;
        this.UserAgent = userAgent;
    }

    public Guid UserId { get; }

    public Ulid Id { get; }

    public IPAddress IpAddress { get; }

    public string UserAgent { get; }

    public abstract byte[] Serialize();
}
