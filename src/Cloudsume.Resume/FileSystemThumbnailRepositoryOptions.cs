namespace Candidate.Server.Resume
{
    using System.ComponentModel.DataAnnotations;

    public sealed class FileSystemThumbnailRepositoryOptions
    {
        [Required]
        public string Path { get; set; } = null!;
    }
}
