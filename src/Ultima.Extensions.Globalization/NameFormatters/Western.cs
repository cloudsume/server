namespace Candidate.Globalization.NameFormatters
{
    internal sealed class Western : NameFormatter
    {
        private Western()
        {
        }

        public static Western Default { get; } = new Western();

        public override string Format(string first, string? middle, string last)
        {
            if (middle != null)
            {
                return $"{first} {middle} {last}";
            }
            else
            {
                return $"{first} {last}";
            }
        }
    }
}
