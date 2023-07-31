namespace Cloudsume.Cassandra.Models;

using System;

public interface IResumeSampleData
{
    Guid Owner { get; set; }

    Guid TargetJob { get; set; }

    string Culture { get; set; }

    DataObject? Data { get; set; }

    Guid? ParentJob { get; set; }
}
