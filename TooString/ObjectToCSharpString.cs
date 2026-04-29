using System.Reflection;

namespace TooString;

public static partial class ObjectTooString
{
    /// <summary>
    /// Stringify <paramref name="value"/> as a C# anonymous object.
    /// The resulting string should be pastable into a C# console or text editor.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="writeIndented">Whether to format with indentation and newlines</param>
    /// <param name="whichProperties">
    /// <see cref="BindingFlags"/> to select properties. Defaults to Instance | Public.
    /// </param>
    /// <param name="maxDepth">Maximum depth for nested objects. Defaults to 3.</param>
    /// <param name="maxEnumerableLength">Maximum number of enumerable elements to include. Defaults to 9.</param>
    /// <param name="dateTimeFormat">DateTime format string. Defaults to "O" (ISO 8601).</param>
    /// <param name="dateOnlyFormat">DateOnly format string. Defaults to "O".</param>
    /// <param name="timeOnlyFormat">TimeOnly format string. Defaults to "HH:mm:ss".</param>
    /// <param name="timeSpanFormat">TimeSpan format string. Defaults to "c".</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A string representation of <paramref name="value"/> in C# anonymous object style.
    /// Type information is included in inline comments.
    /// </returns>
    public static string ToCSharpString<T>(this T value,
                                           bool writeIndented = true,
                                           BindingFlags whichProperties =
                                               BindingFlags.Instance | BindingFlags.Public,
                                           int maxDepth = 3,
                                           int maxEnumerableLength = 9,
                                           string dateTimeFormat = "O",
                                           string dateOnlyFormat = "O",
                                           string timeOnlyFormat = "HH:mm:ss",
                                           string timeSpanFormat = "c")
        => TooString(value, new TooStringOptions
        {
            StringifyAs = StringifyAs.CSharp,
            WriteIndented = writeIndented,
            WhichProperties = whichProperties,
            MaxDepth = maxDepth,
            MaxEnumerationLength = maxEnumerableLength,
            DateTimeFormat = dateTimeFormat,
            DateOnlyFormat = dateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat,
            TimeSpanFormat = timeSpanFormat,
        });

    /// <summary>
    /// Stringify <paramref name="value"/> in a style similar
    /// to the Visual Studio debugger.
    /// </summary>
    /// <param name="value">The value to stringify</param>
    /// <param name="writeIndented">Whether to format with indentation and newlines</param>
    /// <param name="whichProperties">
    /// <see cref="BindingFlags"/> to select properties. Defaults to Instance | Public.
    /// </param>
    /// <param name="maxDepth">Maximum depth for nested objects. Defaults to 3.</param>
    /// <param name="maxEnumerableLength">Maximum number of enumerable elements to include. Defaults to 9.</param>
    /// <param name="dateTimeFormat">DateTime format string. Defaults to "O" (ISO 8601).</param>
    /// <param name="dateOnlyFormat">DateOnly format string. Defaults to "O".</param>
    /// <param name="timeOnlyFormat">TimeOnly format string. Defaults to "HH:mm:ss".</param>
    /// <param name="timeSpanFormat">TimeSpan format string. Defaults to "c".</param>
    /// <returns>
    /// A string representation of <paramref name="value"/> in Visual Studio debugger style.
    /// </returns>
    public static string ToDebugViewString<T>(this T value,
                                              bool writeIndented = true,
                                              BindingFlags whichProperties =
                                                  BindingFlags.Instance | BindingFlags.Public,
                                              int maxDepth = 3,
                                              int maxEnumerableLength = 9,
                                              string dateTimeFormat = "O",
                                              string dateOnlyFormat = "O",
                                              string timeOnlyFormat = "HH:mm:ss",
                                              string timeSpanFormat = "c")
        => TooString(value, new TooStringOptions
        {
            StringifyAs = StringifyAs.DebugView,
            WriteIndented = writeIndented,
            WhichProperties = whichProperties,
            MaxDepth = maxDepth,
            MaxEnumerationLength = maxEnumerableLength,
            DateTimeFormat = dateTimeFormat,
            DateOnlyFormat = dateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat,
            TimeSpanFormat = timeSpanFormat,
        });

}
