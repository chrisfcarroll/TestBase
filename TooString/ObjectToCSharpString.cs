using System.Reflection;

namespace TooString;

public static partial class ObjectTooString
{
    /// <summary>
    /// Stringify <paramref name="value"/> as C# anonymous-object notation
    /// using our reflection-based <see cref="StringifyAs.CSharp"/> style.
    /// </summary>
    public static string ToCSharpString<T>(this T value,
                                           bool writeIndented = false,
                                           BindingFlags whichProperties =
                                               BindingFlags.Instance | BindingFlags.Public,
                                           int maxDepth = 3,
                                           int maxEnumerationLength = 9,
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
            MaxEnumerationLength = maxEnumerationLength,
            DateTimeFormat = dateTimeFormat,
            DateOnlyFormat = dateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat,
            TimeSpanFormat = timeSpanFormat,
        });
}
