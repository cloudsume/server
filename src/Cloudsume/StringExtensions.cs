namespace Candidate.Server
{
    internal static class StringExtensions
    {
        public static string? NullIfEmpty(this string s)
        {
            return s.Length == 0 ? null : s;
        }
    }
}
