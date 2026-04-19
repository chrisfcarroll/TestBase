using System.Reflection;

namespace TooString;

public static partial class ObjectTooString
{
    /// <summary>
    /// Stringify <paramref name="value"/> as JSON using our reflection-based
    /// <see cref="StringifyAs.JsonStringifier"/>.
    /// For pure System.Text.Json serialization, use <see cref="ToSTJson{T}(T?, System.Text.Json.JsonSerializerOptions)"/>.
    /// </summary>
    /// <param name="value">The value to stringify as JSON</param>
    /// <param name="writeIndented">Whether to format with indentation and newlines</param>
    /// <param name="whichProperties">
    /// <see cref="BindingFlags"/> to select properties. Defaults to Instance | Public.
    /// </param>
    /// <param name="maxDepth">Maximum depth for nested objects. Defaults to 3.</param>
    /// <param name="maxEnumerationLength">Maximum number of enumerable elements to include. Defaults to 9.</param>
    /// <param name="dateTimeFormat">DateTime format string. Defaults to "O" (ISO 8601).</param>
    /// <param name="dateOnlyFormat">DateOnly format string. Defaults to "O".</param>
    /// <param name="timeOnlyFormat">TimeOnly format string. Defaults to "HH:mm:ss".</param>
    /// <param name="timeSpanFormat">TimeSpan format string. Defaults to "c".</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A JSON-style string representation of <paramref name="value"/></returns>
    public static string ToJson<T>(this T? value,
                                   bool writeIndented = true,
                                   BindingFlags whichProperties =
                                       BindingFlags.Instance | BindingFlags.Public,
                                   int maxDepth = 3,
                                   int maxEnumerationLength = 9,
                                   string dateTimeFormat = "O",
                                   string dateOnlyFormat = "O",
                                   string timeOnlyFormat = "HH:mm:ss",
                                   string timeSpanFormat = "c")
        => BuildReflectedString(value, OptionsWithState.From(0, new TooStringOptions
        {
            StringifyAs = StringifyAs.JsonStringifier,
            WriteIndented = writeIndented,
            WhichProperties = whichProperties,
            MaxDepth = maxDepth,
            MaxEnumerationLength = maxEnumerationLength,
            DateTimeFormat = dateTimeFormat,
            DateOnlyFormat = dateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat,
            TimeSpanFormat = timeSpanFormat,
        }));
}
