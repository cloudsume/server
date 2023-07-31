namespace Cloudsume.Resume;

using System.Collections.Generic;
using Candidate.Server.Resume;

public interface IParentCollection : IReadOnlyList<IReadOnlyDictionary<GlobalKey, ResumeData>>
{
}
