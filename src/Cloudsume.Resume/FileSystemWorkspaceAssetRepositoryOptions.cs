namespace Cloudsume.Resume
{
    using System.ComponentModel.DataAnnotations;

    public sealed class FileSystemWorkspaceAssetRepositoryOptions
    {
        public FileSystemWorkspaceAssetRepositoryOptions()
        {
            this.Path = string.Empty;
        }

        [Required]
        public string Path { get; set; }
    }
}
