namespace Cloudsume.Models;

using System;
using System.Collections.Generic;
using NetUlid;
using Domain = Cloudsume.Resume.Template;
using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;

public abstract class TemplateInfo
{
    protected TemplateInfo(Domain domain)
    {
        this.Id = domain.Id;
        this.RegistrationId = domain.RegistrationId;
        this.Category = domain.Category;
        this.ApplicableData = domain.ApplicableData;
        this.ReleaseNote = domain.ReleaseNote;
    }

    public Ulid Id { get; }

    public Guid RegistrationId { get; }

    public RegistrationCategory Category { get; }

    public IEnumerable<string> ApplicableData { get; }

    public string ReleaseNote { get; }
}
