namespace Cloudsume.DataOperations;

internal sealed class ContentKey : FormKey
{
    public ContentKey(string id, string? raw)
        : base(raw)
    {
        this.Id = id;
    }

    public string Id { get; }
}
