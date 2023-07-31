namespace Cloudsume.Models
{
    using System.ComponentModel.DataAnnotations;

    public sealed class ReleaseTemplate
    {
        public ReleaseTemplate(string note)
        {
            this.Note = note;
        }

        [MaxLength(10000)]
        public string Note { get; }
    }
}
