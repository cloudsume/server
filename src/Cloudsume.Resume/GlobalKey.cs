namespace Cloudsume.Resume
{
    using System;
    using Candidate.Server.Resume;

    public sealed record GlobalKey
    {
        public GlobalKey(ResumeData source)
        {
            this.Type = source.Type;

            if (source is MultiplicativeData m)
            {
                if (m.Id == null)
                {
                    throw new ArgumentException("The value cannot be an aggregated value.", nameof(source));
                }

                this.Id = m.Id.Value;
            }
        }

        public GlobalKey(string type)
        {
            this.Type = type;
        }

        public GlobalKey(string type, Guid id)
        {
            this.Type = type;
            this.Id = id;
        }

        public string Type { get; }

        public Guid Id { get; }
    }
}
