namespace Cloudsume.Server.Cassandra
{
    using System;

    public interface IMultiplicativeResumeData : IResumeData
    {
        /// <summary>
        /// Gets or sets the index of this data.
        /// </summary>
        /// <remarks>
        /// Always zero for global data.
        /// </remarks>
        public sbyte Index { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this data.
        /// </summary>
        /// <remarks>
        /// The value is always <see cref="Guid.Empty"/> for local data.
        /// </remarks>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data to base on.
        /// </summary>
        /// <remarks>
        /// The value is always <c>null</c> for invariant global data.
        /// </remarks>
        public Guid? BaseId { get; set; }
    }
}
