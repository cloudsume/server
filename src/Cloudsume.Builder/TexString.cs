namespace Cloudsume.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal sealed class TexString
    {
        // Basic Latin.
        private static readonly Rune Circumflex = new('^');
        private static readonly Rune LineFeed = new('\n');
        private static readonly Rune ReverseSolidus = new('\\');
        private static readonly Rune Tilde = new('~');

        private static readonly IReadOnlySet<Rune> NeedEscape = new HashSet<Rune>()
        {
            new('&'),
            new('%'),
            new('$'),
            new('#'),
            new('_'),
            new('{'),
            new('}'),
        };

        public TexString(string s)
        {
            this.Value = s;
        }

        public string Value { get; }

        public static TexString? From(string? s)
        {
            return s == null ? null : new TexString(s);
        }

        /// <summary>
        /// Escape a specified string for LaTeX.
        /// </summary>
        /// <param name="value">
        /// A string to escape.
        /// </param>
        /// <param name="culture">
        /// A culture of LaTeX source.
        /// </param>
        /// <returns>
        /// An escaped string to use in LaTeX source.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not a valid text for LaTeX source.
        /// </exception>
        public static string Transform(string value, CultureInfo culture)
        {
            var result = new StringBuilder();
            var current = CultureInfo.InvariantCulture;
            var start = 0;
            var i = 0;

            while (i < value.Length)
            {
                // Get current rune.
                Rune rune;

                try
                {
                    rune = Rune.GetRuneAt(value, i);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"The value is not a valid text.", nameof(value), ex);
                }

                // Escape LaTeX reserved characters.
                if (NeedEscape.Contains(rune))
                {
                    Flush();
                    result.Append('\\');
                    result.Append(rune.ToString());
                    start += rune.Utf16SequenceLength; // Skip an escapped character.
                }
                else if (rune == Tilde)
                {
                    Flush();
                    result.Append(@"\~{}");
                    start += rune.Utf16SequenceLength; // Skip an escapped character.
                }
                else if (rune == Circumflex)
                {
                    Flush();
                    result.Append(@"\^{}");
                    start += rune.Utf16SequenceLength; // Skip an escapped character.
                }
                else if (rune == ReverseSolidus)
                {
                    Flush();
                    result.Append(@"\textbackslash{}");
                    start += rune.Utf16SequenceLength; // Skip an escapped character.
                }
                else if (rune == LineFeed)
                {
                    // Prevent automatically paragraph with a blank line.
                    Flush();
                    result.Append(@"~\newline{}");
                    start += rune.Utf16SequenceLength; // Skip an escapped character.
                }
                else
                {
                    // Switch culture if the current rune is in a different culture.
                    // We use a performance hack here by compare references due to this method may process a large string.
                    var c = rune.GetCulture();

                    if (c != current)
                    {
                        if (c != CultureInfo.InvariantCulture && c.TwoLetterISOLanguageName != culture.TwoLetterISOLanguageName)
                        {
                            throw new ArgumentException($"Character {rune} is not supported by culture {culture}.", nameof(value));
                        }

                        Flush();
                        current = c;
                    }
                }

                i += rune.Utf16SequenceLength;
            }

            Flush();

            return result.ToString();

            void Flush()
            {
                // Prefix with language specific command.
                switch (current.TwoLetterISOLanguageName)
                {
                    case "th":
                        result.Append(@"\textthai{");
                        break;
                    default:
                        if (current != CultureInfo.InvariantCulture)
                        {
                            throw new NotImplementedException();
                        }

                        break;
                }

                // Write processed string.
                result.Append(value, start, i - start);

                if (current != CultureInfo.InvariantCulture)
                {
                    result.Append('}');
                }

                start = i;
            }
        }
    }
}
