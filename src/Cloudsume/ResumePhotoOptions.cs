namespace Candidate.Server
{
    using Microsoft.Extensions.Configuration;

    internal sealed class ResumePhotoOptions
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
