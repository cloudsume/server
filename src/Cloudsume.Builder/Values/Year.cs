namespace Candidate.Server.Resume.Builder.Values
{
    using System;

    internal sealed class Year : AttributeValue<Year, DateTime>
    {
        public Year(DateTime value)
            : base(value)
        {
        }
    }
}
