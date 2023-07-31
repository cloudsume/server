namespace Cloudsume.DataOperations;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// A service to deserialize data operations for a resume.
/// </summary>
/// <remarks>
/// This service is designed for local data. For global data, use <see cref="IGlobalOperationSerializer"/> instead.
/// </remarks>
public interface IDataOperationSerializer
{
    Task<IEnumerable<DataOperation>?> DeserializeAsync(IFormCollection request, CancellationToken cancellationToken = default);
}
