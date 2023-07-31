namespace Ultima.Extensions.Globalization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public sealed class SubdivisionCodeJsonConverter : JsonConverter<SubdivisionCode>
    {
        public override SubdivisionCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString();

            if (json == null)
            {
                // This should never happens due to framework already handled null for us.
                throw new ArgumentException("The JSON value is null.", nameof(reader));
            }

            try
            {
                return SubdivisionCode.Parse(json);
            }
            catch (FormatException ex)
            {
                throw new JsonException($"'{json}' is not a valid subdivision code.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, SubdivisionCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
