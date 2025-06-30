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
    

    /// <summary>
    /// Transform a json or DebugView string of an Assembly or reflected type so that we can
    /// compare it with an expected value without needing to know exact details such
    /// as file paths and lengths, or version, Guid, or hash values.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>
    /// A “hashed” string in which machine-, time-, hash-, guid- and filesize- dependent values
    /// are replaced with fixed pseudovalues
    /// </returns>
    [return:NotNullIfNotNull("value")]
    public static string? RegexReplaceCompilationDependentValuesWithPseudoValues(this string? value)
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
            .ReplaceRegex("\"DefinedTypes\":\"System.RuntimeType\\[\\d+\\]", "\"DefinedTypes\":\"System.RuntimeType[99]")
            .ReplaceRegex("\"ExportedTypes\":\"System.Type\\[\\d+\\]", "\"ExportedTypes\":\"System.Type[99]")

            .ReplaceRegex(" (/[^\" ,]+)+,"," --filename--,")
            .ReplaceRegex(" file:///[^\" ,]+,"," file:///--filename--,")
            .ReplaceRegex(" file:///[BCD]:(/[^\",]+)+,"," file:///--filename--,")
            .ReplaceRegex(" [BCD]:\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" [BCD]:\\\\[^,\"]+,"," --filename--,")
            .ReplaceRegex(" Value = \\d+"," Value = 9999999999")
            .ReplaceRegex(" Length = \\d+"," Length = 9999999999")
            .ReplaceRegex(" LongLength = \\d+"," LongLength = 9999999999")
            .ReplaceRegex(" MetadataToken = \\d+"," MetadataToken = 100000000")
            .ReplaceRegex(" DefinedTypes = System.RuntimeType\\[\\d+\\]", " DefinedTypes = System.RuntimeType[99]")
            .ReplaceRegex(" ExportedTypes = System.Type\\[\\d+\\]", " ExportedTypes = System.Type[99]")
            .ReplaceRegex(@"Version=\d{1,8}\.\d{1,8}\.\d{1,8}\.\d{1,8}", "Version=X.X.X.X")

            .ReplaceRegex("\\\\u002B[a-f0-9A-F]{40}","+" + new string('a',40))
            .ReplaceRegex("[a-f0-9A-F]{40}",new string('a',40))
            .ReplaceRegex("\\\\u0022","\\\"")
            .ReplaceRegex("[0-9a-fA-F]{8}[-][0-9a-fA-F]{4}[-][0-9a-fA-F]{4}[-][0-9a-fA-F]{4}[-][0-9a-fA-F]{12}",Guid.Empty.ToString())
            .ReplaceRegex("[a-f0-9A-F]{32}",new string('0',32));
}