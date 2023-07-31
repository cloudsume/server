namespace Candidate.Globalization
{
    using System.Collections.Generic;
    using System.Globalization;

    public interface ITranslationCollection : IReadOnlyCollection<KeyValuePair<CultureInfo, string>>
    {
        string CurrentCulture { get; }

        string this[CultureInfo culture] { get; }
    }
}
