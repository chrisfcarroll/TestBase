using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TooString;

/// <summary>
/// Extension methods for Regex Replace.
/// </summary>
static class RegexReplace
{

    [return: NotNullIfNotNull("input")]
    public static string? ReplaceRegex(this string? input,
                                       string pattern,
                                       string replacement,
                                       RegexOptions options = RegexOptions.None)
        => input is null
            ? null
            : Regex.Replace(input, pattern, replacement, options);
    
    [return:NotNullIfNotNull("value")]
    public static string? RegexReplaceKnownRuntimeVariableValues(this string? value)
        => value?
            .ReplaceRegex("\"file:///[^\"]+\"","\"file:///--filename--\"")
            .ReplaceRegex("\"file:///[BCD]:(/[^\",]+)+\",","\"file:///--filename--\",")
            .ReplaceRegex("\"[BCD]:\\[^\"]+\"","\"--filename--\"")
            .ReplaceRegex("\"[BCD]:\\\\[^\"]+\"","\"--filename--\"")
            .ReplaceRegex("\"(/[^/\"]+)+\"","\"--filename--\"")
            .ReplaceRegex("\"Value\":\\d+","\"Value\":9999999999")
            .ReplaceRegex("\"Length\":\\d+","\"Length\":9999999999")
            .ReplaceRegex("\"LongLength\":\\d+","\"LongLength\":9999999999")
            .ReplaceRegex("\"MetadataToken\":\\d+","\"MetadataToken\":100000000")
        
            .ReplaceRegex(" (/[^\" ,]+)+,"," --filename--,")
            .ReplaceRegex(" file:///[^\" ,]+,"," file:///--filename--,")
            .ReplaceRegex(" file:///[BCD]:(/[^\",]+)+,"," file:///--filename--,")
            .ReplaceRegex(" [BCD]:\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" [BCD]:\\\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" Value = \\d+"," Value = 9999999999")
            .ReplaceRegex(" Length = \\d+"," Length = 9999999999")
            .ReplaceRegex(" LongLength = \\d+"," LongLength = 9999999999")
            .ReplaceRegex(" MetadataToken = \\d+"," MetadataToken = 100000000")
        
            .ReplaceRegex("[a-f0-9A-F\\-]{36}",System.Guid.Empty.ToString());
}