namespace Candidate.Server.Resume
{
    using System;

    public abstract class MultiplicativeData : ResumeData
    {
        protected MultiplicativeData(Guid? id, Guid? baseId, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Id = id;
            this.BaseId = baseId;
        }

        /// <summary>
        /// Gets or sets an identifier for this data.
        /// </summary>
        /// <remarks>
        /// The value will be <see cref="Guid.Empty"/> for local data and <see langword="null"/> for agregated data.
        /// </remarks>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets the identifier of the base value.
        /// </summary>
        /// <remarks>
        /// The value is always <c>null</c> for global invariant data.
        /// </remarks>
        public Guid? BaseId { get; }
    }
}
