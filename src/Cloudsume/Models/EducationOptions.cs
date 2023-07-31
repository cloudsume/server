namespace Cloudsume.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Domain = Candidate.Server.Resume.Data.EducationRenderOptions;

    public sealed class EducationOptions
    {
        public EducationOptions(Domain domain)
        {
            this.DescriptionParagraph = domain.DescriptionParagraph;
            this.DescriptionListOptions = domain.DescriptionListOptions;
        }

        [JsonConstructor]
        public EducationOptions(string? descriptionParagraph, string? descriptionListOptions)
        {
            this.DescriptionParagraph = descriptionParagraph;
            this.DescriptionListOptions = descriptionListOptions;
        }

        [MaxLength(1000)]
        public string? DescriptionParagraph { get; }

        [MaxLength(1000)]
        public string? DescriptionListOptions { get; }
    }
}
