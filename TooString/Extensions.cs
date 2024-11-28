using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TooString;

public static class Extensions
{
    public static T With<T>(this T @this, Action<T> with)
    {
        with(@this);
        return @this;
    }
    public static JsonSerializerOptions With(
        this JsonSerializerOptions @this, 
        Action<JsonSerializerOptions> with)
    {
        var copy = new JsonSerializerOptions
        {
            PropertyNamingPolicy = @this.PropertyNamingPolicy,
            IgnoreNullValues = @this.IgnoreNullValues,
            IgnoreReadOnlyProperties = @this.IgnoreReadOnlyProperties,
            WriteIndented = @this.WriteIndented,
            MaxDepth = @this.MaxDepth,
            Encoder = @this.Encoder,
            IncludeFields = @this.IncludeFields,
            NumberHandling = @this.NumberHandling,
            ReferenceHandler = @this.ReferenceHandler,
            AllowTrailingCommas = @this.AllowTrailingCommas,
            DefaultBufferSize = @this.DefaultBufferSize,
            DefaultIgnoreCondition = @this.DefaultIgnoreCondition,
            DictionaryKeyPolicy = @this.DictionaryKeyPolicy,
            ReadCommentHandling = @this.ReadCommentHandling,
            UnknownTypeHandling = @this.UnknownTypeHandling,
            IgnoreReadOnlyFields = @this.IgnoreReadOnlyFields,
            PropertyNameCaseInsensitive = @this.PropertyNameCaseInsensitive
        };
        copy.Converters.AddAll(@this.Converters);
        with(copy);
        return copy;
    }

    public static IList<T> AddAll<T>(this IList<T> @this, IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            @this.Add(item);
        }
        return @this;
    }

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