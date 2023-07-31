namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;

public sealed class Link
{
    public Link(LinkId id, string name, Guid user, Guid resume, IReadOnlySet<LinkCensorship> censorships, DateTime createAt)
    {
        this.Id = id;
        this.Name = name;
        this.User = user;
        this.Resume = resume;
        this.Censorships = censorships;
        this.CreateAt = createAt;
    }

    public LinkId Id { get; }

    public string Name { get; }

    public Guid User { get; }

    public Guid Resume { get; }

    public IReadOnlySet<LinkCensorship> Censorships { get; }

    public DateTime CreateAt { get; }
}
