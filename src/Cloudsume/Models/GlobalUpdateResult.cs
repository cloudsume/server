namespace Cloudsume.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class GlobalUpdateResult
    {
        public GlobalUpdateResult(IEnumerable<ResumeData> updatedData, IEnumerable<Guid> affectedResumes)
        {
            this.UpdatedData = updatedData;
            this.AffectedResumes = affectedResumes;
        }

        public IEnumerable<ResumeData> UpdatedData { get; }

        public IEnumerable<Guid> AffectedResumes { get; }
    }
}
