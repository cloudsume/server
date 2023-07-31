namespace Candidate.Server.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetUlid;

    public sealed record Resume : ResumeInfo
    {
        public Resume(
            Guid userId,
            Guid id,
            string name,
            Ulid templateId,
            bool recruitmentConsent,
            DateTime createdAt,
            DateTime? updatedAt)
            : this(userId, id, name, templateId, recruitmentConsent, createdAt, updatedAt, Enumerable.Empty<ResumeData>())
        {
        }

        public Resume(
            Guid userId,
            Guid id,
            string name,
            Ulid templateId,
            bool recruitmentConsent,
            DateTime createdAt,
            DateTime? updatedAt,
            IEnumerable<ResumeData> data)
            : base(userId, id, name, templateId, recruitmentConsent, createdAt, updatedAt)
        {
            this.Data = data;
        }

        public IEnumerable<ResumeData> Data { get; init; }
    }
}
