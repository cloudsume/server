namespace Cloudsume.Models;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Domain = Cloudsume.Resume.Link;
using LinkCensorship = Cloudsume.Resume.LinkCensorship;

public sealed class ResumeLink
{
    public ResumeLink(Domain domain, DateTime? accessedAt)
    {
        this.Id = WebEncoders.Base64UrlEncode(domain.Id.Value);
        this.Name = domain.Name;
        this.Censorships = domain.Censorships;
        this.AccessedAt = accessedAt;
        this.CreatedAt = domain.CreateAt;
    }

    public string Id { get; }

    public string Name { get; }

    public IReadOnlySet<LinkCensorship> Censorships { get; }

    public DateTime? AccessedAt { get; }

    public DateTime CreatedAt { get; }
}
