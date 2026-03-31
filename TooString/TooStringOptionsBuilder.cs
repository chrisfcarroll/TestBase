using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TooString;

/// <summary>
/// A fluent builder for constructing <see cref="TooStringOptions"/>.
/// <example>
/// <code>
/// var options = new TooStringOptionsBuilder()
///     .UseJson()
///     .WriteIndented()
///     .Build();
///
/// var options2 = new TooStringOptionsBuilder()
///     .UseDebugView()
///     .MaxDepth(5)
///     .MaxLength(20)
///     .Build();
/// </code>
/// </example>
/// </summary>
public class TooStringOptionsBuilder
{
    TooStringStyle style = TooStringStyle.JsonSerializer;
    TooStringStyle reflectionStyle = TooStringStyle.DebugView;
    int maxDepth = 3;
    int maxEnumerationLength = 9;
    bool writeIndented;
    BindingFlags whichProperties = BindingFlags.Instance | BindingFlags.Public;
    string dateTimeFormat = "O";
    string dateOnlyFormat = "O";
    string timeOnlyFormat = "HH:mm:ss";
    string timeSpanFormat = "c";
    JsonSerializerOptions? jsonOptions;

    /// <summary>Use <see cref="TooStringStyle.JsonSerializer"/> (System.Text.Json serialization)</summary>
    public TooStringOptionsBuilder UseJson()
    {
        style = TooStringStyle.JsonSerializer;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.DebugView"/> (reflection with <c>{ A = "B" }</c> style)</summary>
    public TooStringOptionsBuilder UseDebugView()
    {
        style = TooStringStyle.DebugView;
        reflectionStyle = TooStringStyle.DebugView;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.JsonStringifier"/> (reflection with <c>{"A":"B"}</c> style)</summary>
    public TooStringOptionsBuilder UseReflectionJson()
    {
        style = TooStringStyle.JsonStringifier;
        reflectionStyle = TooStringStyle.JsonStringifier;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.CSharp"/> (reflection with <c>/*Type*/ new { A = "B" }</c> style)</summary>
    public TooStringOptionsBuilder UseCSharp()
    {
        style = TooStringStyle.CSharp;
        reflectionStyle = TooStringStyle.CSharp;
        return this;
    }

    /// <summary>Set the maximum depth for nested object traversal</summary>
    public TooStringOptionsBuilder MaxDepth(int maxDepth)
    {
        this.maxDepth = maxDepth;
        return this;
    }

    /// <summary>Set the maximum number of enumerable items to stringify</summary>
    public TooStringOptionsBuilder MaxLength(int maxLength)
    {
        maxEnumerationLength = maxLength;
        return this;
    }

    /// <summary>Enable indented output (multiline)</summary>
    public TooStringOptionsBuilder WriteIndented(bool indented = true)
    {
        writeIndented = indented;
        return this;
    }

    /// <summary>Set which properties to include via <see cref="BindingFlags"/></summary>
    public TooStringOptionsBuilder WhichProperties(BindingFlags flags)
    {
        whichProperties = flags;
        return this;
    }

    /// <summary>Set the DateTime format string</summary>
    public TooStringOptionsBuilder DateTimeFormat(string format)
    {
        dateTimeFormat = format;
        return this;
    }

    /// <summary>Set the DateOnly format string</summary>
    public TooStringOptionsBuilder DateOnlyFormat(string format)
    {
        dateOnlyFormat = format;
        return this;
    }

    /// <summary>Set the TimeOnly format string</summary>
    public TooStringOptionsBuilder TimeOnlyFormat(string format)
    {
        timeOnlyFormat = format;
        return this;
    }

    /// <summary>Set the TimeSpan format string</summary>
    public TooStringOptionsBuilder TimeSpanFormat(string format)
    {
        timeSpanFormat = format;
        return this;
    }

    /// <summary>Supply custom <see cref="JsonSerializerOptions"/> for JSON serialization</summary>
    public TooStringOptionsBuilder JsonOptions(JsonSerializerOptions options)
    {
        jsonOptions = options;
        return this;
    }

    /// <summary>Configure custom <see cref="JsonSerializerOptions"/> via an action</summary>
    public TooStringOptionsBuilder JsonOptions(Action<JsonSerializerOptions> configure)
    {
        jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        configure(jsonOptions);
        return this;
    }

    /// <summary>Build the <see cref="TooStringOptions"/> instance</summary>
    public TooStringOptions Build()
    {
        var jsonOpts = jsonOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        if (writeIndented) jsonOpts = jsonOpts.With(o => o.WriteIndented = true);

        return new TooStringOptions
        {
            JsonOptions = jsonOpts,
            StringifyAs = style,
            WhichProperties = whichProperties,
            MaxDepth = maxDepth,
            MaxEnumerationLength = maxEnumerationLength,
            DateTimeFormat = dateTimeFormat,
            DateOnlyFormat = dateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat,
            TimeSpanFormat = timeSpanFormat,
        };
    }
}
