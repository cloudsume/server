namespace Cloudsume.DataOperations;

public class UpdateLocalData : DataOperation
{
    public UpdateLocalData(string key, Candidate.Server.Resume.ResumeData update)
        : base(key)
    {
        this.Update = update;
    }

    public Candidate.Server.Resume.ResumeData Update { get; }

    public void Deconstruct(out string key, out Candidate.Server.Resume.ResumeData update)
    {
        key = this.Key;
        update = this.Update;
    }
}
