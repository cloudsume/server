namespace Cloudsume.DataOperations;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// A service to deserialize sample data operations.
/// </summary>
/// <remarks>
/// For resume data, use <see cref="IDataOperationSerializer"/> instead.
/// </remarks>
public interface ISampleOperationSerializer
{
    Task<IEnumerable<DataOperation>?> DeserializeAsync(Guid jobId, CultureInfo culture, IFormCollection request, CancellationToken cancellationToken = default);
}
