namespace Cloudsume.Server.Models
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Candidate.Server;
    using static Candidate.Server.Resume.Data.ILR;

    public sealed class ResumeLanguageProficiencyConverter : JsonConverter<ResumeLanguageProficiency>
    {
        public override ResumeLanguageProficiency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Read and check raw value.
            using var json = JsonDocument.ParseValue(ref reader);
            var root = json.RootElement;

            switch (root.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Object:
                    break;
                default:
                    throw new JsonException("Unsupported value.");
            }

            // Get proficiency type.
            ResumeLanguageProficiencyType type;

            if (root.TryGetProperty(options.NormalizeProperty(nameof(ResumeLanguageProficiency.Type)), out var value))
            {
                try
                {
                    type = (ResumeLanguageProficiencyType)value.GetInt32();
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is FormatException)
                {
                    throw new JsonException("Invalid type.", ex);
                }
            }
            else
            {
                type = default;
            }

            // Get proficiency value.
            object proficiency;

            if (root.TryGetProperty(options.NormalizeProperty(nameof(ResumeLanguageProficiency.Value)), out value))
            {
                try
                {
                    proficiency = type switch
                    {
                        ResumeLanguageProficiencyType.ILR => (ScaleId)value.GetInt32(),
                        ResumeLanguageProficiencyType.TOEIC => value.GetInt32(),
                        ResumeLanguageProficiencyType.IELTS => JsonSerializer.Deserialize<ResumeIelts>(value.GetRawText(), options) ?? new object(),
                        ResumeLanguageProficiencyType.TOEFL => value.GetInt32(),
                        _ => new object(),
                    };
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is FormatException)
                {
                    throw new JsonException("Invalid value.", ex);
                }
            }
            else
            {
                proficiency = new();
            }

            return new(type, proficiency);
        }

        public override void Write(Utf8JsonWriter writer, ResumeLanguageProficiency value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(options.NormalizeProperty(nameof(value.Type)));
            JsonSerializer.Serialize(writer, value.Type, options);

            writer.WritePropertyName(options.NormalizeProperty(nameof(value.Value)));
            JsonSerializer.Serialize(writer, value.Value, options);

            writer.WriteEndObject();
        }
    }
}
