namespace Cloudsume.Aws
{
    using System.ComponentModel.DataAnnotations;

    public class S3RepositoryOptions
    {
        public S3RepositoryOptions()
        {
            this.Bucket = string.Empty;
        }

        [Required]
        public string Bucket { get; set; }
    }
}
