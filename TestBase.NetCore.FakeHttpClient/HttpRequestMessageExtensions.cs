using System.Text.Json;

namespace TestBase;

/// <summary>
/// Extension methods for reading HttpRequestMessage content in tests.
/// </summary>
public static class HttpRequestMessageExtensions
{
    /// <summary>Read the request body as a string.</summary>
    public static string? ReadContentAsString(this HttpRequestMessage request)
        => request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();

    /// <summary>Read the request body as a deserialized JSON object.</summary>
    public static T? ReadContentAsJson<T>(this HttpRequestMessage request, JsonSerializerOptions? options = null)
    {
        var content = request.ReadContentAsString();
        return content is null
            ? default
            : JsonSerializer.Deserialize<T>(content, options ?? JsonReadOptions.Default);
    }

    static class JsonReadOptions
    {
        internal static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>Check if the request has a header with the given value.</summary>
    public static bool HasHeader(this HttpRequestMessage request, string name, string value)
        => request.Headers.TryGetValues(name, out var values) && values.Contains(value);

    /// <summary>Check if the request has a header (any value).</summary>
    public static bool HasHeader(this HttpRequestMessage request, string name)
        => request.Headers.Contains(name);
}
