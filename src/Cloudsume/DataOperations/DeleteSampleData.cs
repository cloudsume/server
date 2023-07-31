namespace Cloudsume.DataOperations;

public class DeleteSampleData : DataOperation
{
    public DeleteSampleData(string key, string type)
        : base(key)
    {
        this.Type = type;
    }

    public string Type { get; }

    public void Deconstruct(out string key, out string type)
    {
        key = this.Key;
        type = this.Type;
    }
}
