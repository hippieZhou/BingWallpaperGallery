// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinggoWallpapers.Core.Helpers;

internal class DateTimeConverter(string format) : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String &&
            DateTime.TryParseExact(reader.GetString(), format, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new JsonException("Invalid date format");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}

