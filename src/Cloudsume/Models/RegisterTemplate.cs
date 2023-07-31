namespace Cloudsume.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using Candidate.Server;
    using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;

    public sealed class RegisterTemplate
    {
        public RegisterTemplate(
            string name,
            string? description,
            Uri? website,
            CultureInfo culture,
            IReadOnlyCollection<Guid> applicableJobs,
            Guid? previewJob,
            RegistrationCategory category)
        {
            this.Name = name;
            this.Description = description;
            this.Website = website;
            this.Culture = culture;
            this.ApplicableJobs = applicableJobs;
            this.PreviewJob = previewJob;
            this.Category = category;
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; }

        [StringLength(10000, MinimumLength = 1)]
        public string? Description { get; }

        [Uri(Schemes = UriSchemes.HTTP | UriSchemes.HTTPS, Kind = UriKind.Absolute)]
        public Uri? Website { get; }

        [Required]
        public CultureInfo Culture { get; }

        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public IReadOnlyCollection<Guid> ApplicableJobs { get; }

        public Guid? PreviewJob { get; }

        [RequireDefined]
        public RegistrationCategory Category { get; }
    }
}
