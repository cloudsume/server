namespace Cloudsume.Models
{
    using System;
    using NetUlid;
    using Domain = Cloudsume.Resume.TemplateLicense;
    using TemplateLicenseStatus = Cloudsume.Template.TemplateLicenseStatus;

    public sealed class TemplateLicense
    {
        public TemplateLicense(Domain domain)
        {
            this.Id = domain.Id;
            this.RegistrationId = domain.RegistrationId;
            this.UserId = domain.UserId;
            this.Status = domain.Status;
            this.UpdatedAt = domain.UpdatedAt;
        }

        public Ulid Id { get; }

        public Guid RegistrationId { get; }

        public Guid UserId { get; }

        public TemplateLicenseStatus Status { get; }

        public DateTime? UpdatedAt { get; }
    }
}
