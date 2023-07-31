namespace Cloudsume.Resume
{
    using System.ComponentModel.DataAnnotations;

    public sealed class FileSystemPreviewRepositoryOptions
    {
        public FileSystemPreviewRepositoryOptions()
        {
            this.Path = string.Empty;
        }

        [Required]
        public string Path { get; set; }
    }
}
