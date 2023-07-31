namespace Cloudsume.Resume
{
    using System.ComponentModel.DataAnnotations;

    public sealed class FileSystemWorkspacePreviewRepositoryOptions
    {
        public FileSystemWorkspacePreviewRepositoryOptions()
        {
            this.Path = string.Empty;
        }

        [Required]
        public string Path { get; set; }
    }
}
