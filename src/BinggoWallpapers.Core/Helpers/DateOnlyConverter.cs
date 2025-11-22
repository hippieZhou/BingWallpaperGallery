// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinggoWallpapers.Core.Helpers;

internal class DateOnlyConverter(string format) : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && DateOnly.TryParseExact(reader.GetString(), format, out var date))
        {
            return date;
        }

        throw new JsonException("Invalid date format");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}
