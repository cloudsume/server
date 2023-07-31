namespace Cloudsume.Data
{
    using System;
    using Candidate.Globalization;

    public sealed class Job
    {
        public Job(Guid id, ITranslationCollection names)
        {
            this.Id = id;
            this.Names = names;
        }

        public Guid Id { get; }

        public ITranslationCollection Names { get; }
    }
}
