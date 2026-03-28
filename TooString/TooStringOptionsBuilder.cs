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
    TooStringStyle _style = TooStringStyle.BestEffort;
    ReflectionStyle _reflectionStyle = ReflectionStyle.DebugView;
    int _maxDepth = 3;
    int _maxEnumerationLength = 9;
    bool _writeIndented;
    BindingFlags _whichProperties = BindingFlags.Instance | BindingFlags.Public;
    string _dateTimeFormat = "O";
    string _dateOnlyFormat = "O";
    string _timeOnlyFormat = "HH:mm:ss";
    string _timeSpanFormat = "c";
    JsonSerializerOptions? _jsonOptions;
    readonly List<TooStringStyle> _fallbacks = new();

    /// <summary>Use <see cref="TooStringStyle.JsonSerializer"/> (System.Text.Json serialization)</summary>
    public TooStringOptionsBuilder UseJson()
    {
        _style = TooStringStyle.JsonSerializer;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.DebugView"/> (reflection with <c>{ A = "B" }</c> style)</summary>
    public TooStringOptionsBuilder UseDebugView()
    {
        _style = TooStringStyle.DebugView;
        _reflectionStyle = ReflectionStyle.DebugView;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.JsonStringifier"/> (reflection with <c>{"A":"B"}</c> style)</summary>
    public TooStringOptionsBuilder UseReflectionJson()
    {
        _style = TooStringStyle.JsonStringifier;
        _reflectionStyle = ReflectionStyle.Json;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.CSharp"/> (reflection with <c>/*Type*/ new { A = "B" }</c> style)</summary>
    public TooStringOptionsBuilder UseCSharp()
    {
        _style = TooStringStyle.CSharp;
        _reflectionStyle = ReflectionStyle.CSharp;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.CallerArgument"/></summary>
    public TooStringOptionsBuilder UseCallerArgument()
    {
        _style = TooStringStyle.CallerArgument;
        return this;
    }

    /// <summary>Use <see cref="TooStringStyle.ToString"/></summary>
    public TooStringOptionsBuilder UseToString()
    {
        _style = TooStringStyle.ToString;
        return this;
    }

    /// <summary>Set the maximum depth for nested object traversal</summary>
    public TooStringOptionsBuilder MaxDepth(int maxDepth)
    {
        _maxDepth = maxDepth;
        return this;
    }

    /// <summary>Set the maximum number of enumerable items to stringify</summary>
    public TooStringOptionsBuilder MaxLength(int maxLength)
    {
        _maxEnumerationLength = maxLength;
        return this;
    }

    /// <summary>Enable indented output (multiline)</summary>
    public TooStringOptionsBuilder WriteIndented(bool indented = true)
    {
        _writeIndented = indented;
        return this;
    }

    /// <summary>Set which properties to include via <see cref="BindingFlags"/></summary>
    public TooStringOptionsBuilder WhichProperties(BindingFlags flags)
    {
        _whichProperties = flags;
        return this;
    }

    /// <summary>Set the DateTime format string</summary>
    public TooStringOptionsBuilder DateTimeFormat(string format)
    {
        _dateTimeFormat = format;
        return this;
    }

    /// <summary>Set the DateOnly format string</summary>
    public TooStringOptionsBuilder DateOnlyFormat(string format)
    {
        _dateOnlyFormat = format;
        return this;
    }

    /// <summary>Set the TimeOnly format string</summary>
    public TooStringOptionsBuilder TimeOnlyFormat(string format)
    {
        _timeOnlyFormat = format;
        return this;
    }

    /// <summary>Set the TimeSpan format string</summary>
    public TooStringOptionsBuilder TimeSpanFormat(string format)
    {
        _timeSpanFormat = format;
        return this;
    }

    /// <summary>Supply custom <see cref="JsonSerializerOptions"/> for JSON serialization</summary>
    public TooStringOptionsBuilder JsonOptions(JsonSerializerOptions options)
    {
        _jsonOptions = options;
        return this;
    }

    /// <summary>Configure custom <see cref="JsonSerializerOptions"/> via an action</summary>
    public TooStringOptionsBuilder JsonOptions(Action<JsonSerializerOptions> configure)
    {
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        configure(_jsonOptions);
        return this;
    }

    /// <summary>Add a fallback style to try if the primary style fails</summary>
    public TooStringOptionsBuilder FallbackTo(TooStringStyle style)
    {
        _fallbacks.Add(style);
        return this;
    }

    /// <summary>Build the <see cref="TooStringOptions"/> instance</summary>
    public TooStringOptions Build()
    {
        var jsonOpts = _jsonOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        if (_writeIndented) jsonOpts = jsonOpts.With(o => o.WriteIndented = true);

        var reflectionOpts = new ReflectionOptions(
            WhichProperties: _whichProperties,
            Style: _reflectionStyle,
            MaxDepth: _maxDepth,
            MaxEnumerationLength: _maxEnumerationLength,
            DateTimeFormat: _dateTimeFormat,
            DateOnlyFormat: _dateOnlyFormat,
            TimeOnlyFormat: _timeOnlyFormat,
            TimeSpanFormat: _timeSpanFormat);

        var fallbacks = new List<TooStringStyle> { _style };
        fallbacks.AddRange(_fallbacks);
        fallbacks.Add(TooStringStyle.BestEffort);

        return new TooStringOptions(reflectionOpts, jsonOpts, fallbacks.AsReadOnly());
    }
}
