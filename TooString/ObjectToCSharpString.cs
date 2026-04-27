using System.Reflection;

namespace TooString;

public static partial class ObjectTooString
{
    /// <summary>
    /// Stringify <paramref name="value"/> as a C# anonymous object.
    /// The resulting string should be pastable into a C# console or text editor.
    /// </summary>
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
