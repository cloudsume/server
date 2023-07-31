namespace Cloudsume.DataOperations;

public sealed class UpdateGlobalData : DataOperation
{
    public UpdateGlobalData(string key, Candidate.Server.Resume.ResumeData update)
        : base(key)
    {
        this.Update = update;
    }

    public Candidate.Server.Resume.ResumeData Update { get; }
}
