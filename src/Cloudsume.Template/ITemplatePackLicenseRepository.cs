namespace Cloudsume.Template;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface ITemplatePackLicenseRepository
{
    Task CreateAsync(TemplatePackLicense license, CancellationToken cancellationToken = default);

    Task<TemplatePackLicense?> GetAsync(Guid userId, Guid packId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplatePackLicense>> ListAsync(Guid packId, CancellationToken cancellationToken = default);
}
