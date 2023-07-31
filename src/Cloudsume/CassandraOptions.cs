namespace Cloudsume
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using global::Cassandra;
    using Microsoft.Extensions.Configuration;

    internal sealed class CassandraOptions
    {
        public CassandraOptions()
        {
            this.ContactPoints = Array.Empty<string>();
            this.Keyspace = string.Empty;
            this.Username = string.Empty;
            this.ReadConsistencies = new();
        }

        public enum PasswordProvider
        {
            Inline,
            AWS,
        }

        [Required]
        [MinLength(1)]
        public string[] ContactPoints { get; set; }

        [Required]
        public string Keyspace { get; set; }

        public string Username { get; set; }

        public PasswordOptions? Password { get; set; }

        public ReadConsistencyOptions ReadConsistencies { get; set; }

        public sealed class PasswordOptions
        {
            public PasswordProvider Provider { get; set; }

            public IConfigurationSection? ProviderOptions { get; set; }
        }

        public sealed class InlinePassword
        {
            public string? Password { get; set; }
        }

        public sealed class AwsPassword
        {
            public string? SecretId { get; set; }
        }

        public sealed class ReadConsistencyOptions
        {
            public ConsistencyLevel StrongConsistency { get; set; }
        }
    }
}
