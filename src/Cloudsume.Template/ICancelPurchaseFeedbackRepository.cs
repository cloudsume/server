namespace Cloudsume.Template;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetUlid;

public interface ICancelPurchaseFeedbackRepository
{
    Task CreateAsync(CancelPurchaseFeedback feedback, CancellationToken cancellationToken = default);

    Task<IEnumerable<CancelPurchaseFeedback>> ListAsync(Guid templateId, Ulid? skipTill = null, int max = 100, CancellationToken cancellationToken = default);
}
