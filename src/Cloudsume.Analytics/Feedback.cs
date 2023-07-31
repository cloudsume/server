namespace Cloudsume.Analytics;

using System;
using System.Net;
using System.Net.Mail;
using NetUlid;

public sealed class Feedback
{
    public const int MinScore = 0;
    public const int MaxScore = 9;

    public Feedback(Ulid id, int? score, string detail, MailAddress? contact, Guid? userId, IPAddress ipAddress, string userAgent)
    {
        if (score.HasValue && (score.Value < MinScore || score.Value > MaxScore))
        {
            throw new ArgumentOutOfRangeException(nameof(score));
        }

        this.Id = id;
        this.Score = score;
        this.Detail = detail;
        this.Contact = contact;
        this.UserId = userId;
        this.IpAddress = ipAddress;
        this.UserAgent = userAgent;
    }

    public Ulid Id { get; }

    public int? Score { get; }

    public string Detail { get; }

    public MailAddress? Contact { get; }

    public Guid? UserId { get; }

    public IPAddress IpAddress { get; }

    public string UserAgent { get; }
}
