namespace Candidate.Globalization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using Ultima.Extensions.Globalization;

    [JsonConverter(typeof(TranslationCollectionJsonConverter))]
    public sealed class TranslationCollection : ITranslationCollection
    {
        private readonly Dictionary<CultureInfo, string> translations;

        public TranslationCollection()
        {
            this.translations = new();
        }

        public TranslationCollection(IEnumerable<KeyValuePair<string, string>> source)
            : this()
        {
            foreach (var (locale, name) in source)
            {
                this.translations.Add(CultureInfo.GetCultureInfo(locale), name);
            }
        }

        public int Count => this.translations.Count;

        public string CurrentCulture => this[CultureInfo.CurrentCulture];

        public string this[CultureInfo culture]
        {
            get
            {
                for (var current = culture; ; current = current.Parent)
                {
                    if (this.translations.TryGetValue(current, out var t))
                    {
                        return t;
                    }

                    if (current.Equals(CultureInfo.InvariantCulture))
                    {
                        break;
                    }
                }

                throw new TranslationNotFoundException($"No translation for '{culture}'.");
            }
        }

        /// <summary>
        /// Add a new translation.
        /// </summary>
        /// <param name="culture">
        /// The culture of the translation.
        /// </param>
        /// <param name="value">
        /// The translation.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Translation with the same <paramref name="culture"/> already exists.
        /// </exception>
        public void Add(CultureInfo culture, string value) => this.translations.Add(culture, value);

        /// <summary>
        /// Add a new translation.
        /// </summary>
        /// <param name="culture">
        /// The culture of the translation.
        /// </param>
        /// <param name="value">
        /// The translation.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="culture"/> is not a valid culture name or already exists.
        /// </exception>
        public void Add(string culture, string value)
        {
            try
            {
                this.Add(CultureInfo.GetCultureInfo(culture), value);
            }
            catch (CultureNotFoundException ex)
            {
                throw new ArgumentException("The value is not a valid culture name.", nameof(culture), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("The translation for this culture already exists.", nameof(culture), ex);
            }
        }

        public IEnumerator<KeyValuePair<CultureInfo, string>> GetEnumerator() => this.translations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
