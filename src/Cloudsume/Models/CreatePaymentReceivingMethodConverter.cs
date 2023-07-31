namespace Cloudsume.Models
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Candidate.Server;

    public sealed class CreatePaymentReceivingMethodConverter : JsonConverter<CreatePaymentReceivingMethod>
    {
        public override CreatePaymentReceivingMethod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Load value into memory.
            using var json = JsonDocument.ParseValue(ref reader);
            var root = json.RootElement;

            switch (root.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Object:
                    break;
                default:
                    throw new JsonException("The value is not a valid request to create a payment provider.");
            }

            // Read payload.
            var type = root.GetEnum<PaymentProvider>(options.NormalizeProperty(nameof(CreatePaymentReceivingMethod.Type)));

            return type switch
            {
                PaymentProvider.Stripe => this.ReadStripe(root, options),
                _ => throw new JsonException($"Unknown provider type {type}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, CreatePaymentReceivingMethod value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Common.
            writer.WriteNumber(options.NormalizeProperty(nameof(value.Type)), Convert.ToInt32(value.Type));

            // Provider specifics.
            switch (value)
            {
                case CreateStripeReceivingMethod v:
                    writer.WriteString(options.NormalizeProperty(nameof(v.Country)), v.Country.Name);
                    break;
                default:
                    throw new NotImplementedException($"Provider {value.GetType()} is not implemented.");
            }

            writer.WriteEndObject();
        }

        private CreateStripeReceivingMethod ReadStripe(in JsonElement root, JsonSerializerOptions options)
        {
            // Country.
            RegionInfo country;

            try
            {
                country = new(root.GetString(options.NormalizeProperty(nameof(CreateStripeReceivingMethod.Country))));
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("Invalid country.", ex);
            }

            return new(country);
        }
    }
}
