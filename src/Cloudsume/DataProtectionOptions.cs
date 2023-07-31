namespace Candidate.Server
{
    using Microsoft.Extensions.Configuration;

    internal sealed class DataProtectionOptions
    {
        public enum ProviderType
        {
            Default,
            FileSystem,
            AWS,
        }

        public ProviderType Provider { get; set; }

        public IConfigurationSection? ProviderOptions { get; set; }

        public sealed class FileSystemOptions
        {
            public string? Path { get; set; }
        }

        public sealed class AwsOptions
        {
            public string? Path { get; set; }
        }
    }
}
