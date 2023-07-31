namespace Cloudsume.Models
{
    using System;
    using Domain = Cloudsume.Resume.TemplateAsset;

    public sealed class TemplateAsset
    {
        public TemplateAsset(Domain domain)
        {
            this.Name = domain.Name.Value;
            this.Size = domain.Size;
            this.LastModified = domain.LastModified;
        }

        public string Name { get; }

        public int Size { get; }

        public DateTime LastModified { get; }
    }
}
