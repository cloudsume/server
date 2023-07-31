namespace Cloudsume.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using LinkCensorship = Cloudsume.Resume.LinkCensorship;

    public sealed class CreateResumeLink
    {
        [JsonConstructor]
        public CreateResumeLink(string name, HashSet<LinkCensorship>? censorships)
        {
            this.Name = name;
            this.Censorships = censorships;
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; }

        public HashSet<LinkCensorship>? Censorships { get; }
    }
}
