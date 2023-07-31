namespace Cloudsume.DataOperations;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// A service to deserialize global data.
/// </summary>
/// <remarks>
/// This service is designed for global data. For local data, use <see cref="IDataOperationSerializer"/> instead.
/// </remarks>
public interface IGlobalOperationSerializer
{
    Task<IEnumerable<DataOperation>?> DeserializeAsync(IFormCollection request, CancellationToken cancellationToken = default);
}
