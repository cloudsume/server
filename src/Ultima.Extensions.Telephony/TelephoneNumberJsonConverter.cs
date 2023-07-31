namespace Ultima.Extensions.Telephony
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public sealed class TelephoneNumberJsonConverter : JsonConverter<TelephoneNumber>
    {
        public override TelephoneNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<Model>(ref reader, options);
            RegionInfo country;

            if (dto == null)
            {
                return null;
            }

            if (dto.Country == null || dto.Number == null)
            {
                throw new JsonException("The value is not a valid data for phone number.");
            }

            try
            {
                country = new(dto.Country);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("Invalid country identifier.", ex);
            }

            try
            {
                return new(country, dto.Number);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The number is not a valid national number.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, TelephoneNumber value, JsonSerializerOptions options)
        {
            var dto = new Model()
            {
                Country = value.Country.Name,
                Number = value.Number,
            };

            JsonSerializer.Serialize(writer, dto, options);
        }

        private sealed class Model
        {
            public string? Country { get; set; }

            public string? Number { get; set; }
        }
    }
}
