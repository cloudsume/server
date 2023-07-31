namespace Cloudsume.Resume;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class LinkCensorshipJsonConverter : JsonConverter<LinkCensorship>
{
    public override LinkCensorship? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = reader.GetString();

        if (json == null)
        {
            return null;
        }

        try
        {
            return LinkCensorship.Parse(json);
        }
        catch (FormatException ex)
        {
            throw new JsonException("Invalid censorship identifier.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, LinkCensorship value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
