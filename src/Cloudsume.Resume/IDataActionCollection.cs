namespace Cloudsume.Resume;

using System.Collections.Generic;

public interface IDataActionCollection<T> : IReadOnlyDictionary<string, T> where T : IDataAction
{
}
