namespace Cloudsume.Models
{
    using System.Collections.Generic;
    using Domain = Cloudsume.Resume.Template;

    public sealed class Template : TemplateInfo
    {
        public Template(Domain domain, IEnumerable<string> previews)
            : base(domain)
        {
            this.Previews = previews;
        }

        public IEnumerable<string> Previews { get; }
    }
}
