namespace Cloudsume.Server.Cassandra
{
    using System;
    using Cloudsume.Cassandra.Models;

    public interface IResumeData
    {
        /// <summary>
        /// Gets or sets the owner of this data.
        /// </summary>
        Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the resume this data belong to.
        /// </summary>
        /// <remarks>
        /// Always <see cref="Guid.Empty"/> for global data.
        /// </remarks>
        Guid ResumeId { get; set; }

        /// <summary>
        /// Gets or sets the language of this data.
        /// </summary>
        /// <remarks>
        /// Always empty for resume-specific data.
        /// </remarks>
        string Language { get; set; }

        DataObject? Data { get; set; }
    }
}
