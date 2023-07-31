namespace Cloudsume.Models;

using System;
using System.Collections.Generic;
using NetUlid;
using Domain = Candidate.Server.Resume.ResumeInfo;

public class ResumeInfo
{
    public ResumeInfo(Domain domain, IEnumerable<ResumeLink> links)
    {
        this.Id = domain.Id;
        this.Name = domain.Name;
        this.Template = domain.TemplateId;
        this.CreatedAt = domain.CreatedAt;
        this.Links = links;
        this.RecruitmentConsent = domain.RecruitmentConsent;
    }

    public Guid Id { get; }

    public string Name { get; }

    public Ulid Template { get; }

    public IEnumerable<ResumeLink> Links { get; }

    public bool RecruitmentConsent { get; }

    public DateTime CreatedAt { get; }
}
