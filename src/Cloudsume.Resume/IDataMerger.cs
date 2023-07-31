namespace Cloudsume.Resume;

using Candidate.Server.Resume;

internal interface IDataMerger : IDataAction
{
    ResumeData Merge(ResumeData top, IParentCollection parents);
}
