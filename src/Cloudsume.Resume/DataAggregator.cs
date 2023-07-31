namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using Candidate.Server.Resume;

    internal sealed class DataAggregator : IDataAggregator
    {
        private readonly IDataActionCollection<IDataMerger> mergers;

        public DataAggregator(IDataActionCollection<IDataMerger> mergers)
        {
            this.mergers = mergers;
        }

        public IEnumerable<ResumeData> Aggregate(IEnumerable<ResumeData> tops, IParentCollection parents)
        {
            var result = new List<ResumeData>();

            foreach (var top in tops)
            {
                result.Add(this.mergers[top.Type].Merge(top, parents));
            }

            return result;
        }

        public T Aggregate<T>(T top, IParentCollection parents) where T : ResumeData
        {
            return (T)this.mergers[top.Type].Merge(top, parents);
        }
    }
}
