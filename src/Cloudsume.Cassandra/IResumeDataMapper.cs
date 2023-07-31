namespace Cloudsume.Cassandra;

using System;
using System.Globalization;
using Candidate.Server.Resume;
using Cloudsume.Resume;

internal interface IResumeDataMapper : IDataAction
{
    Models.DataObject ToCassandra(ResumeData domain);

    /// <summary>
    /// Create a domain object from cassandra object.
    /// </summary>
    /// <param name="id">
    /// Unique identifier of <paramref name="cassandra"/>. Specify <see cref="Guid.Empty"/> for singular data (e.g. name).
    /// </param>
    /// <param name="parent">
    /// Unique identifier of the parent for <paramref name="cassandra"/>. Specify <c>null</c> for singular data (e.g. name).
    /// </param>
    /// <param name="cassandra">
    /// The object to create a domain object.
    /// </param>
    /// <returns>
    /// The domain object for <paramref name="cassandra"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="cassandra"/> contain invalid data.
    /// </exception>
    ResumeData ToDomain(Guid id, Guid? parent, Models.DataObject cassandra);

    GlobalData CreateGlobal(CultureInfo culture, ResumeData data);
}
