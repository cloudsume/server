namespace Candidate.Server
{
    using Microsoft.Extensions.Configuration;

    public sealed class ResumeThumbnailOptions
    {
        public enum RepositoryType
        {
            FileSystem,
            AWS,
        }

        public RepositoryType Repository { get; set; }

        public IConfigurationSection? RepositoryOptions { get; set; }
    }
}
