namespace Cloudsume.DataOperations;

public class UpdateSampleData : DataOperation
{
    public UpdateSampleData(string key, Cloudsume.Resume.SampleData update)
        : base(key)
    {
        this.Update = update;
    }

    public Cloudsume.Resume.SampleData Update { get; }

    public void Deconstruct(out string key, out Cloudsume.Resume.SampleData update)
    {
        key = this.Key;
        update = this.Update;
    }
}
