namespace Cloudsume.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;

internal abstract class UniqueAttributeFactory<T> : AttributeFactory, IAttributeFactory where T : ResumeData
{
    public Type TargetData => typeof(T);

    public virtual ValueTask<object?> CreateAsync(BuildContext context, T data, CancellationToken cancellationToken = default)
    {
        return new(this.Create(context, data));
    }

    ValueTask<object?> IAttributeFactory.CreateAsync(BuildContext context, IEnumerable<ResumeData> data, CancellationToken cancellationToken)
    {
        return this.CreateAsync(context, (T)data.Single(), cancellationToken);
    }

    protected virtual object? Create(BuildContext context, T data)
    {
        throw new NotImplementedException($"The derived class must override either '{nameof(this.Create)}' or '{nameof(this.CreateAsync)}'.");
    }
}
