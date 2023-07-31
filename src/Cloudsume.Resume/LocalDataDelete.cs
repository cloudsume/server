namespace Candidate.Server.Resume
{
    public sealed class LocalDataDelete
    {
        public LocalDataDelete(string type, int? index = default)
        {
            this.Type = type;
            this.Index = index;
        }

        public string Type { get; }

        public int? Index { get; }
    }
}
