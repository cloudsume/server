namespace Cloudsume.Server.Models
{
    using System;
    using Domain = Candidate.Server.Resume.MultiplicativeData;

    public abstract class MultiplicativeData
    {
        protected MultiplicativeData(Guid id, Guid? @base)
        {
            this.Id = id;
            this.Base = @base;
        }

        protected MultiplicativeData(Domain domain)
        {
            if (domain.Id == null)
            {
                throw new ArgumentException("Aggregated data is not supported.", nameof(domain));
            }

            this.Id = domain.Id.Value;
            this.Base = domain.BaseId;
        }

        /// <summary>
        /// Gets the identifier of this data.
        /// </summary>
        /// <remarks>
        /// Always <see cref="Guid.Empty"/> for local data. <see cref="Guid.Empty"/> for global data mean create a new entry.
        /// </remarks>
        public Guid Id { get; }

        public Guid? Base { get; }
    }
}
