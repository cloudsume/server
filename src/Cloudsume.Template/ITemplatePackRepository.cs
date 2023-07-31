namespace Cloudsume.Template;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface ITemplatePackRepository
{
    Task CreateAsync(TemplatePack pack, CancellationToken cancellationToken = default);

    Task<TemplatePack?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplatePack>> GetPacksAsync(Guid registrationId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplatePack>> ListAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplatePack>> ListAsync(Guid userId, CancellationToken cancellationToken = default);
}
