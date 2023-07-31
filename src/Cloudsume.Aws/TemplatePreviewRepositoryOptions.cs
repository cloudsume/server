namespace Candidate.Server.Aws
{
    using System.ComponentModel.DataAnnotations;

    public sealed class TemplatePreviewRepositoryOptions
    {
        public TemplatePreviewRepositoryOptions()
        {
            this.Bucket = string.Empty;
        }

        [Required]
        public string Bucket { get; set; }
    }
}
