namespace Ultima.Extensions.Globalization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents an ISO 3166-2.
    /// </summary>
    [JsonConverter(typeof(SubdivisionCodeJsonConverter))]
    [TypeConverter(typeof(SubdivisionCodeConverter))]
    public readonly struct SubdivisionCode : IEquatable<SubdivisionCode>
    {
        public static readonly SubdivisionCode Null = default;

        private readonly string? value;

        private SubdivisionCode(string value)
        {
            this.value = value;
        }

        public string Country => this.value?.Substring(0, 2) ?? throw new InvalidOperationException("The object is null.");

        public string Subdivision => this.value?.Substring(3) ?? throw new InvalidOperationException("The object is null.");

        public string Value => this.value ?? throw new InvalidOperationException("The object is null.");

        public static bool operator ==(SubdivisionCode a, SubdivisionCode b) => a.Equals(b);

        public static bool operator !=(SubdivisionCode a, SubdivisionCode b) => !a.Equals(b);

        /// <summary>
        /// Convert a specified string contains ISO 3166-2 into <see cref="SubdivisionCode"/>.
        /// </summary>
        /// <param name="s">
        /// A string containing ISO 3166-2.
        /// </param>
        /// <returns>
        /// A <see cref="SubdivisionCode"/> represent the same value as <paramref name="s"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is not a valid ISO 3166-2.
        /// </exception>
        public static SubdivisionCode Parse(string s)
        {
            // Check length.
            var length = s.Length;

            if (length < 4 || length > 6)
            {
                throw new FormatException();
            }

            // Check country format.
            if (s[2] != '-' || !IsAlpha(s[0]) || !IsAlpha(s[1]))
            {
                throw new FormatException();
            }

            // Check subdivision format.
            if (!IsAlphanumeric(s[3]) || (length >= 5 && !IsAlphanumeric(s[4])) || (length >= 6 && !IsAlphanumeric(s[5])))
            {
                throw new FormatException();
            }

            // Check if valid.
            if (!SubdivisionNames.Table.ContainsKey(s))
            {
                throw new FormatException();
            }

            return new(s);
        }

        public bool Equals(SubdivisionCode other)
        {
            return other.value == this.value;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((SubdivisionCode)obj);
        }

        public override int GetHashCode()
        {
            return this.value != null ? this.value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return this.value ?? string.Empty;
        }

        public string ToString(CultureInfo culture)
        {
            var names = SubdivisionNames.Table[this.Value]; // We want to force exception here.

            if (!names.TryGetValue(culture.TwoLetterISOLanguageName, out var name))
            {
                throw new ArgumentException("Unsupported culture.", nameof(culture));
            }

            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAlpha(char ch) => ch >= 0x41 && ch <= 0x5A;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAlphanumeric(char ch) => IsAlpha(ch) || (ch >= 0x30 && ch <= 0x39);
    }
}
