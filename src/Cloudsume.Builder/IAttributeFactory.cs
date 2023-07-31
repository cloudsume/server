namespace Cloudsume.Builder;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cloudsume.Resume;

public interface IAttributeFactory : IDataAction
{
    ValueTask<object?> CreateAsync(BuildContext context, IEnumerable<ResumeData> data, CancellationToken cancellationToken = default);
}
