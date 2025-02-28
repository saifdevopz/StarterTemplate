using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorTemplate.Common.Helpers;

internal static class Serialization
{
    public static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
        };
    }

    public static string SerializeObj<T>(T modelObject)
    {
        return JsonSerializer.Serialize(modelObject, JsonOptions());
    }

    public static T DeserializeJsonString<T>(string jsonString)
    {
        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
    }

    public static IList<T> DeserializeJsonStringList<T>(string jsonString)
    {
        return JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;
    }

    public static StringContent GenerateStringContent(string serialiazedObj)
    {
        return new(serialiazedObj, System.Text.Encoding.UTF8, "application/json");
    }
}