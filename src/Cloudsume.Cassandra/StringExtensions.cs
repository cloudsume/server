namespace Cloudsume.Cassandra
{
    internal static class StringExtensions
    {
        public static string? NullOnEmpty(this string s) => string.IsNullOrWhiteSpace(s) ? null : s;
    }
}
