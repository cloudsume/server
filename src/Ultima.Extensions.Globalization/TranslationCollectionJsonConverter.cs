namespace Ultima.Extensions.Globalization
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Candidate.Globalization;

    public sealed class TranslationCollectionJsonConverter : JsonConverter<TranslationCollection>
    {
        public override TranslationCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = new TranslationCollection();

            for (; ;)
            {
                if (!reader.Read())
                {
                    throw new JsonException();
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                // Load culture name.
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                CultureInfo culture;

                try
                {
                    culture = CultureInfo.GetCultureInfo(reader.GetString()!);
                }
                catch (CultureNotFoundException ex)
                {
                    throw new JsonException("Unknown language.", ex);
                }

                // Load translation.
                if (!reader.Read() || reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                try
                {
                    result.Add(culture, reader.GetString()!);
                }
                catch (ArgumentException ex)
                {
                    throw new JsonException("Duplicated language.", ex);
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, TranslationCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var (culture, translation) in value)
            {
                writer.WriteString(culture.Name, translation);
            }

            writer.WriteEndObject();
        }
    }
}
