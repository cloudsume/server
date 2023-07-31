namespace Cloudsume.Configurations;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IConfigurationRepository
{
    Task SetSlackUriAsync(Uri? uri, CancellationToken cancellationToken = default);

    Task<Uri?> GetSlackUriAsync(CancellationToken cancellationToken = default);
}
