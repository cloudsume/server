namespace Cloudsume.Server.Models
{
    using System.Collections.Generic;
    using Cloudsume.Models;

    public sealed class DataUpdateResult
    {
        public DataUpdateResult(IEnumerable<string> thumbnails, IEnumerable<ResumeData> data)
        {
            this.Thumbnails = thumbnails;
            this.Data = data;
        }

        public IEnumerable<string> Thumbnails { get; }

        public IEnumerable<ResumeData> Data { get; }
    }
}
