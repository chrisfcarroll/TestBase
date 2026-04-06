using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString;

public static partial class ObjectTooString
{
    /// <summary>
    /// Serialize <paramref name="value"/> using <see cref="JsonSerializer"/>.
    /// This is a pure System.Text.Json call with no TooString fallback.
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="jsonSerializerOptions">
    /// Options to pass to <see cref="JsonSerializer.Serialize{TValue}(TValue, JsonSerializerOptions?)"/>.
    /// </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The JSON string produced by System.Text.Json</returns>
    public static string ToSTJson<T>(this T? value, JsonSerializerOptions jsonSerializerOptions)
        => JsonSerializer.Serialize(value, jsonSerializerOptions);

    /// <summary>
    /// Serialize <paramref name="value"/> using <see cref="JsonSerializer"/>
    /// with individually specified options.
    /// This is a pure System.Text.Json call with no TooString fallback.
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="writeIndented">Whether to pretty-print the JSON output</param>
    /// <param name="propertyNamingPolicy">
    /// The naming policy for property names (e.g. <see cref="JsonNamingPolicy.CamelCase"/>).
    /// Defaults to null (property names unchanged).
    /// </param>
    /// <param name="defaultIgnoreCondition">
    /// When to ignore properties during serialization.
    /// Defaults to <see cref="JsonIgnoreCondition.Never"/>.
    /// </param>
    /// <param name="numberHandling">
    /// How numbers are handled during serialization.
    /// Defaults to <see cref="JsonNumberHandling.Strict"/>.
    /// </param>
    /// <param name="referenceHandler">
    /// How object references are handled. Defaults to null (no reference handling).
    /// </param>
    /// <param name="propertyNameCaseInsensitive">
    /// Whether property name matching is case-insensitive.
    /// </param>
    /// <param name="includeFields">Whether to include fields in serialization</param>
    /// <param name="maxDepth">
    /// The maximum depth for serialization. 0 means default (64).
    /// </param>
    /// <param name="allowTrailingCommas">Whether trailing commas are allowed when deserializing</param>
    /// <param name="readCommentHandling">How JSON comments are handled during deserialization</param>
    /// <param name="encoder">The encoder to use for escaping strings</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The JSON string produced by System.Text.Json</returns>
    public static string ToSTJson<T>(this T? value,
                                     bool writeIndented = false,
                                     JsonNamingPolicy? propertyNamingPolicy = null,
                                     JsonIgnoreCondition defaultIgnoreCondition = JsonIgnoreCondition.Never,
                                     JsonNumberHandling numberHandling = JsonNumberHandling.Strict,
                                     ReferenceHandler? referenceHandler = null,
                                     bool propertyNameCaseInsensitive = false,
                                     bool includeFields = false,
                                     int maxDepth = 0,
                                     bool allowTrailingCommas = false,
                                     JsonCommentHandling readCommentHandling = JsonCommentHandling.Disallow,
                                     System.Text.Encodings.Web.JavaScriptEncoder? encoder = null)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = writeIndented,
            PropertyNamingPolicy = propertyNamingPolicy,
            DefaultIgnoreCondition = defaultIgnoreCondition,
            NumberHandling = numberHandling,
            ReferenceHandler = referenceHandler,
            PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
            IncludeFields = includeFields,
            MaxDepth = maxDepth,
            AllowTrailingCommas = allowTrailingCommas,
            ReadCommentHandling = readCommentHandling,
            Encoder = encoder,
        };
        return JsonSerializer.Serialize(value, options);
    }
}
