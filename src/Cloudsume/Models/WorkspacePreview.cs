namespace Cloudsume.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public sealed class WorkspacePreview
    {
        public WorkspacePreview(string? source, IEnumerable<string> thumbnails)
        {
            this.Source = source;
            this.Thumbnails = thumbnails;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Source { get; }

        public IEnumerable<string> Thumbnails { get; }
    }
}
