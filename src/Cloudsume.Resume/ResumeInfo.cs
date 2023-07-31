namespace Candidate.Server.Resume
{
    using System;
    using NetUlid;

    public record ResumeInfo
    {
        public ResumeInfo(Guid userId, Guid id, string name, Ulid templateId, bool recruitmentConsent, DateTime createdAt, DateTime? updatedAt)
        {
            this.Id = id;
            this.UserId = userId;
            this.Name = name;
            this.TemplateId = templateId;
            this.RecruitmentConsent = recruitmentConsent;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;
        }

        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public string Name { get; init; }

        public Ulid TemplateId { get; init; }

        public bool RecruitmentConsent { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }

        public bool Equals(Resume? other)
        {
            if (other == null || other.EqualityContract != this.EqualityContract)
            {
                return false;
            }

            return other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.EqualityContract, this.Id);
        }
    }
}
