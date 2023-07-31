namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using Candidate.Server.Resume;

    public interface IDataAggregator
    {
        IEnumerable<ResumeData> Aggregate(IEnumerable<ResumeData> tops, IParentCollection parents);

        T Aggregate<T>(T top, IParentCollection parents) where T : ResumeData;
    }
}
