#nullable disable

namespace Candidate.Server.Resume.Data.Repositories
{
    using System.ComponentModel.DataAnnotations;

    public sealed class PhotoImageRepositoryOptions
    {
        [Required]
        public string Path { get; set; }
    }
}
