namespace Candidate.Server.Resume
{
    public class LocalDataUpdate
    {
        public LocalDataUpdate(ResumeData value)
        {
            this.Value = value;
        }

        public ResumeData Value { get; }
    }
}
