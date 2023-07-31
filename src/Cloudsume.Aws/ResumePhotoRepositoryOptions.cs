#nullable disable

namespace Candidate.Server.Aws
{
    using System.ComponentModel.DataAnnotations;

    public sealed class ResumePhotoRepositoryOptions
    {
        [Required]
        public string Bucket { get; set; }
    }
}
