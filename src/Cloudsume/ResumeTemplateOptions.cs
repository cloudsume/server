namespace Candidate.Server
{
    using Microsoft.Extensions.Configuration;

    public sealed class ResumeTemplateOptions
    {
        public enum AssetRepositoryType
        {
            FileSystem,
            AWS,
        }

        public enum PreviewRepositoryType
        {
            FileSystem,
            AWS,
        }

        public AssetRepositoryType AssetRepository { get; set; }

        public IConfigurationSection? AssetRepositoryOptions { get; set; }

        public PreviewRepositoryType PreviewRepository { get; set; }

        public IConfigurationSection? PreviewRepositoryOptions { get; set; }
    }
}
