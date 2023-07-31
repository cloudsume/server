namespace Candidate.Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Cloudsume.Models;
    using Domain = Candidate.Server.Resume.Resume;

    public sealed class Resume : ResumeInfo
    {
        public Resume(Domain resume, CultureInfo culture, IEnumerable<string> thumbnails, IEnumerable<ResumeData> data, IEnumerable<ResumeLink> links)
            : base(resume, links)
        {
            this.Culture = culture;
            this.UpdatedAt = resume.UpdatedAt;
            this.Thumbnails = thumbnails;
            this.Data = data;
        }

        public CultureInfo Culture { get; }

        public DateTime? UpdatedAt { get; }

        public IEnumerable<string> Thumbnails { get; }

        public IEnumerable<ResumeData> Data { get; }
    }
}
