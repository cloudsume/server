namespace Cloudsume.Models
{
    using System.ComponentModel.DataAnnotations;
    using NetUlid;

    public sealed class CreateResume
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        public Ulid? Template { get; set; }
    }
}
