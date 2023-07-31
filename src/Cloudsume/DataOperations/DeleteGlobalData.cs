namespace Cloudsume.DataOperations;

public class DeleteGlobalData : DataOperation
{
    public DeleteGlobalData(string key, string type)
        : base(key)
    {
        this.Type = type;
    }

    public string Type { get; }
}
