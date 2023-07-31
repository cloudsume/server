namespace Cloudsume.Models;

using System.Collections.Generic;
using Domain = Candidate.Server.Resume.Resume;

public sealed class ResumeSummary : ResumeInfo
{
    public ResumeSummary(Domain domain, string image, IEnumerable<ResumeLink> links)
        : base(domain, links)
    {
        this.Image = image;
    }

    public string Image { get; set; }
}
