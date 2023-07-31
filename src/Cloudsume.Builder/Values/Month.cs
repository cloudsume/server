namespace Candidate.Server.Resume.Builder.Values
{
    using System;

    internal sealed class Month : AttributeValue<Month, DateTime>
    {
        public Month(DateTime value)
            : base(value)
        {
        }
    }
}
