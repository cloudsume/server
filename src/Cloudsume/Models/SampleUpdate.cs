namespace Cloudsume.Models;

using System;

public sealed class SampleUpdate<T> : ISampleUpdate where T : notnull
{
    public SampleUpdate(Guid? parentJob, T update)
    {
        this.ParentJob = parentJob;
        this.Update = update;
    }

    public Guid? ParentJob { get; }

    public T Update { get; }

    object ISampleUpdate.Update => this.Update;
}
