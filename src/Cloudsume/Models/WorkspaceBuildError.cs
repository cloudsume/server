namespace Cloudsume.Models
{
    public sealed class WorkspaceBuildError
    {
        public WorkspaceBuildError(string? source, string log, int? line)
        {
            this.Source = source;
            this.Log = log;
            this.Line = line;
        }

        public string? Source { get; }

        public string Log { get; }

        public int? Line { get; }
    }
}
