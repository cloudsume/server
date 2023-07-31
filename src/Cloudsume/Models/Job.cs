namespace Cloudsume.Models
{
    using System;
    using Domain = Cloudsume.Data.Job;

    public sealed class Job
    {
        public Job(Domain domain)
        {
            this.Id = domain.Id;
            this.Name = domain.Names.CurrentCulture;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
