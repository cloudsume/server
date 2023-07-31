namespace Cloudsume.Cassandra.Models;

using System;

public interface IMultiplicableSampleData : IResumeSampleData
{
    Guid Id { get; set; }

    sbyte Position { get; set; }

    Guid? Parent { get; set; }
}
