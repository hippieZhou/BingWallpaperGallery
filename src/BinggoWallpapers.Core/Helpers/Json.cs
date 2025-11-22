// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Helpers;

public static class Json
{
    private static readonly JsonSerializerOptions _options;
    static Json()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _options.Converters.Add(new DateOnlyConverter("yyyyMMdd"));
        _options.Converters.Add(new DateTimeConverter("yyyyMMddHHmm"));
        _options.Converters.Add(new JsonStringEnumConverter<MarketCode>());
        _options.Converters.Add(new JsonStringEnumConverter<ResolutionCode>());
    }

    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run<T>(() => ToObject<T>(value));
    }

    public static T ToObject<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, _options);
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run<string>(() => Stringify(value));
    }

    public static string Stringify(object value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}
