using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly:InternalsVisibleTo("TooString.Specs")]

namespace TooString;

/// <summary>
/// The complete set of stringification options.
/// Use <c>new TooStringOptions { StringifyAs = ..., WriteIndented = true, MaxDepth = 5 }</c>
/// or start from a preset like <see cref="ForJson"/> or <see cref="ForCSharp"/>
/// and customise with a <c>with</c> expression.
/// </summary>
public record TooStringOptions
{
    /// <summary>
    /// Parameterless constructor with sensible defaults.
    /// All properties are settable via init or set, so you can use
    /// <c>new TooStringOptions { StringifyAs = StringifyAs.JsonSerializer, MaxDepth = 5 }</c>
    /// </summary>
    public TooStringOptions()
    {
        JsonOptions = DefaultJsonSerializerOptions;
        WriteIndented = true;
        StringifyAs = StringifyAs.CSharp;
        WhichProperties = BindingFlags.Instance | BindingFlags.Public;
        MaxDepth = 3;
        MaxEnumerationLength = 9;
        DateTimeFormat = "O";
        DateOnlyFormat = "O";
        TimeOnlyFormat = "HH:mm:ss";
        TimeSpanFormat = "c";
    }

    /// <summary><c>
    /// new(JsonSerializerDefaults.General)
    /// {
    ///    ReferenceHandler = ReferenceHandler.IgnoreCycles
    /// }</c>
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
        new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

    /// <summary>
    /// Preset for CSharp-style output via reflection.
    /// Customise further with <c>with { ... }</c>.
    /// </summary>
    public static readonly TooStringOptions ForCSharp = new() ;

    /// <summary>
    /// Default options: CSharp style, default JsonSerializerOptions, MaxDepth 3, MaxEnumerationLength 9.
    /// </summary>
    public static readonly TooStringOptions Default = ForCSharp;

    /// <summary>
    /// Preset for JSON serialization via <see cref="System.Text.Json.JsonSerializer"/>.
    /// Customize further with <c>with { ... }</c>.
    /// </summary>
    public static readonly TooStringOptions ForJson
        = Default with
        {
            StringifyAs = StringifyAs.STJsonSerialization,
        };

    // ──────────────────────────────────────────────
    //  Properties
    // ──────────────────────────────────────────────

    /// <summary>
    /// The <see cref="TooString.StringifyAs"/> to stringify to.
    /// </summary>
    public StringifyAs StringifyAs { get; init; }

    /// <summary>
    /// Gets or sets whether output should use indents and newlines.
    /// Defaults to true.
    /// </summary>
    public bool WriteIndented { get; init; }

    /// <summary>
    /// Options for System.Text.Json serialization.
    /// Only used when <see cref="StringifyAs"/> is <see cref="TooString.StringifyAs.STJsonSerialization"/>.
    /// For pure System.Text.Json calls, use <see cref="ObjectTooString.ToSTJson{T}(T?, JsonSerializerOptions)"/> directly.
    /// </summary>
    public JsonSerializerOptions JsonOptions { get; init; }

    /// <summary>
    /// <see cref="BindingFlags"/> to pick out the properties and fields to stringify via reflection.
    /// Defaults to <c>BindingFlags.Instance | BindingFlags.Public</c>.
    /// </summary>
    public BindingFlags WhichProperties { get; init; }

    /// <summary>
    /// How deep into nested structures should we print before stopping the recursion?
    /// Defaults to 3.
    /// </summary>
    public int MaxDepth { get; init; }

    /// <summary>
    /// How many elements of an Array or other IEnumerable should we print before stopping the loop?
    /// Defaults to 9.
    /// <para><b>NB MaxEnumerationLength = 0 does not mean carry on for ever</b>.
    /// Use MaxEnumerationLength = int.MaxValue for that.
    /// MaxEnumerationLength = 0 means don't print any elements of an enumerable.</para>
    /// <para><b>Negative MaxEnumerationLength</b> will start at the positive length given,
    /// then count down as depth is descended. For instance, MaxEnumerationLength = -2
    /// will use a length of 2 at depth 1, but of zero at depth 3.</para>
    /// </summary>
    public int MaxEnumerationLength { get; init; }

    /// <summary>
    /// The preferred <see cref="DateTime.ToString()"/> format. Defaults to "O" (ISO 8601).
    /// </summary>
    public string DateTimeFormat { get; init; }

    /// <summary>
    /// The preferred <see cref="DateOnly.ToString()"/> format. Defaults to "O".
    /// </summary>
    public string DateOnlyFormat { get; init; }

    /// <summary>
    /// The preferred <see cref="TimeOnly.ToString()"/> format. Defaults to "HH:mm:ss".
    /// </summary>
    public string TimeOnlyFormat { get; init; }

    /// <summary>
    /// The preferred <see cref="TimeSpan.ToString()"/> format. Defaults to "c".
    /// </summary>
    public string TimeSpanFormat { get; init; }

    // ──────────────────────────────────────────────
    //  With() — single nullable-params overload
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns a copy of the current options with any supplied parameters overridden.
    /// Parameters left as <c>null</c> retain their current value.
    /// </summary>
    public TooStringOptions With(
        bool? writeIndented = null,
        StringifyAs? stringifyAs = null,
        BindingFlags? whichProperties = null,
        int? maxDepth = null,
        int? maxEnumerableLength = null,
        string? dateTimeFormat = null,
        string? dateOnlyFormat = null,
        string? timeOnlyFormat = null,
        string? timeSpanFormat = null)
    {
        return this with
        {
            StringifyAs = stringifyAs ?? StringifyAs,
            WriteIndented = writeIndented ?? WriteIndented,
            WhichProperties = whichProperties ?? WhichProperties,
            MaxDepth = maxDepth ?? MaxDepth,
            MaxEnumerationLength = maxEnumerableLength ?? MaxEnumerationLength,
            DateTimeFormat = dateTimeFormat ?? DateTimeFormat,
            DateOnlyFormat = dateOnlyFormat ?? DateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat ?? TimeOnlyFormat,
            TimeSpanFormat = timeSpanFormat ?? TimeSpanFormat,
        };
    }

    // ──────────────────────────────────────────────
    //  Implicit operator
    // ──────────────────────────────────────────────

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;
}

internal record OptionsWithState : TooStringOptions
{
    public int Depth { get; init; }

    internal static OptionsWithState From(int depth, TooStringOptions from)
        => new()
        {
            Depth = depth,
            WriteIndented = from.WriteIndented,
            JsonOptions = from.JsonOptions,
            StringifyAs = from.StringifyAs,
            WhichProperties = from.WhichProperties,
            MaxDepth = from.MaxDepth,
            MaxEnumerationLength = from.MaxEnumerationLength,
            DateTimeFormat = from.DateTimeFormat,
            DateOnlyFormat = from.DateOnlyFormat,
            TimeOnlyFormat = from.TimeOnlyFormat,
            TimeSpanFormat = from.TimeSpanFormat,
        };
}
