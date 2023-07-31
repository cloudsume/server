namespace Cloudsume.Aws;

using System.ComponentModel.DataAnnotations;

public sealed class StatsRepositoryOptions
{
    [Required]
    public string Namespace { get; set; } = string.Empty;
}
