namespace Cloudsume;

using System;

public interface ISampleUpdate
{
    Guid? ParentJob { get; }

    object Update { get; }
}
