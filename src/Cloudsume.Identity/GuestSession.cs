namespace Cloudsume.Identity;

using System;
using System.Net;

public sealed class GuestSession
{
    public GuestSession(Guid userId, string issuer, string keyId, IPAddress requester, DateTime createdAt, Guid? transferredTo, DateTime? transferredAt)
    {
        this.UserId = userId;
        this.Issuer = issuer;
        this.KeyId = keyId;
        this.Requester = requester;
        this.CreatedAt = createdAt;
        this.TransferredTo = transferredTo;
        this.TransferredAt = transferredAt;
    }

    public Guid UserId { get; }

    public string Issuer { get; }

    public string KeyId { get; }

    public IPAddress Requester { get; }

    public DateTime CreatedAt { get; }

    public Guid? TransferredTo { get; }

    public DateTime? TransferredAt { get; }
}
