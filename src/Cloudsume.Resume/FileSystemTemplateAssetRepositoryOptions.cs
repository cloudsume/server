namespace Cloudsume.Resume
{
    using System.ComponentModel.DataAnnotations;

    public sealed class FileSystemTemplateAssetRepositoryOptions
    {
        public FileSystemTemplateAssetRepositoryOptions()
        {
            this.Path = string.Empty;
        }

        [Required]
        public string Path { get; set; }
    }
}
