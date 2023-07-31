namespace Cloudsume.Models
{
    using System;

    public sealed class ResumeData
    {
        public ResumeData(string type, DateTime updatedAt, object? value)
        {
            this.Type = type;
            this.UpdatedAt = updatedAt;
            this.Value = value;
        }

        public string Type { get; set; }

        public DateTime UpdatedAt { get; set; }

        public object? Value { get; set; }
    }
}
