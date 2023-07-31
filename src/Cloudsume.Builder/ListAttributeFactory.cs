namespace Cloudsume.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;

internal abstract class ListAttributeFactory<T> : AttributeFactory, IAttributeFactory where T : MultiplicativeData
{
    public Type TargetData => typeof(T);

    public virtual async ValueTask<object?> CreateAsync(BuildContext context, IEnumerable<T> data, CancellationToken cancellationToken)
    {
        var result = new List<object?>();

        foreach (var entry in data)
        {
            result.Add(await this.CreateAsync(context, entry, cancellationToken));
        }

        return result;
    }

    ValueTask<object?> IAttributeFactory.CreateAsync(BuildContext context, IEnumerable<ResumeData> data, CancellationToken cancellationToken)
    {
        return this.CreateAsync(context, data.Cast<T>(), cancellationToken);
    }

    protected virtual object? Create(BuildContext context, T data)
    {
        throw new NotImplementedException($"The derived class need to override either '{nameof(this.Create)}' or '{nameof(this.CreateAsync)}'.");
    }

    protected virtual ValueTask<object?> CreateAsync(BuildContext context, T data, CancellationToken cancellationToken)
    {
        return new(this.Create(context, data));
    }
}
