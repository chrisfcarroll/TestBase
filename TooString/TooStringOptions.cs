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
/// and customise with <see cref="With(TooStringOptions)"/> or
/// <see cref="With(bool?,TooString.StringifyAs?,BindingFlags?,int?,int?,string?,string?,string?,string?)"/>.
/// </summary>
public record TooStringOptions
{
    /// <summary>
    /// Parameterless constructor with sensible defaults.
    /// All properties are settable via init or set, so you can use
    /// <c>new TooStringOptions { StringifyAs = TooStringStyle.JsonSerializer, MaxDepth = 5 }</c>
    /// </summary>
    public TooStringOptions()
    {
        jsonOptions = DefaultJsonSerializerOptions;
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
    /// Default options: CSharp style, default JsonSerializerOptions, MaxDepth 3, MaxEnumerationLength 9.
    /// </summary>
    public static readonly TooStringOptions Default = new();

    /// <summary>
    /// Preset for JSON serialization via <see cref="System.Text.Json.JsonSerializer"/>.
    /// Customise further with <c>.With(...)</c> or <c>with { ... }</c>.
    /// </summary>
    public static TooStringOptions ForJson
        => Default with { StringifyAs = StringifyAs.STJsonSerialization };

    /// <summary>
    /// Preset for CSharp-style output via reflection.
    /// Customise further with <c>.With(...)</c> or <c>with { ... }</c>.
    /// </summary>
    public static TooStringOptions ForCSharp
        => Default with { StringifyAs = StringifyAs.CSharp };

    // ──────────────────────────────────────────────
    //  Properties
    // ──────────────────────────────────────────────

    /// <summary>
    /// The <see cref="TooString.StringifyAs"/> to stringify to.
    /// </summary>
    public StringifyAs StringifyAs { get; init; }

    /// <summary>
    /// Gets or sets whether output should use indents and newlines.
    /// Defaults to false
    /// </summary>
    public bool WriteIndented
    {
        get => JsonOptions.WriteIndented;
        init
        {
            if (JsonOptions.WriteIndented != value)
            {
                jsonOptions = new JsonSerializerOptions(JsonOptions) { WriteIndented = value };
            }
        }
    }

    JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Options for System.Text.Json serialization.
    /// </summary>
    /// <remarks>
    /// ⚠️Only used when <see cref="StringifyAs"/> is <see cref="TooString.StringifyAs.STJsonSerialization"/>
    /// </remarks>
    public JsonSerializerOptions JsonOptions
    {
        get => jsonOptions;
        init => jsonOptions = value;
    }

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
    //  With() overloads
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns a copy of the current options reconfigured by applying the
    /// <paramref name="reconfigure"/> action.
    /// </summary>
    public TooStringOptions With(Action<TooStringOptions> reconfigure)
    {
        var mutated = this with { };
        reconfigure(mutated);
        return mutated;
    }

    /// <summary>
    /// Returns a copy of the current options with all values replaced by
    /// those from <paramref name="other"/>.
    /// Useful for programmatic composition of option sets.
    /// </summary>
    public TooStringOptions With(TooStringOptions other)
        => this with
        {
            JsonOptions = other.JsonOptions,
            StringifyAs = other.StringifyAs,
            WhichProperties = other.WhichProperties,
            MaxDepth = other.MaxDepth,
            MaxEnumerationLength = other.MaxEnumerationLength,
            DateTimeFormat = other.DateTimeFormat,
            DateOnlyFormat = other.DateOnlyFormat,
            TimeOnlyFormat = other.TimeOnlyFormat,
            TimeSpanFormat = other.TimeSpanFormat,
        };

    /// <summary>
    /// Returns a copy of the current options with any supplied parameters overridden.
    /// Parameters left as <c>null</c> retain their current value.
    /// </summary>
    public TooStringOptions With(
        bool? writeIndented = null,
        StringifyAs? stringifyAs = null,
        BindingFlags? whichProperties = null,
        int? maxDepth = null,
        int? maxEnumerationLength = null,
        string? dateTimeFormat = null,
        string? dateOnlyFormat = null,
        string? timeOnlyFormat = null,
        string? timeSpanFormat = null)
    {
        var result = this with
        {
            StringifyAs = stringifyAs ?? StringifyAs,
            WriteIndented = writeIndented ?? WriteIndented,
            WhichProperties = whichProperties ?? WhichProperties,
            MaxDepth = maxDepth ?? MaxDepth,
            MaxEnumerationLength = maxEnumerationLength ?? MaxEnumerationLength,
            DateTimeFormat = dateTimeFormat ?? DateTimeFormat,
            DateOnlyFormat = dateOnlyFormat ?? DateOnlyFormat,
            TimeOnlyFormat = timeOnlyFormat ?? TimeOnlyFormat,
            TimeSpanFormat = timeSpanFormat ?? TimeSpanFormat,
        };
        return result;
    }

    // ──────────────────────────────────────────────
    //  Implicit operators
    // ──────────────────────────────────────────────

    /// <summary>
    /// Extract the <see cref="JsonOptions"/> out of <paramref name="o"/>
    /// </summary>
    public static implicit operator JsonSerializerOptions(TooStringOptions o)
        => o.JsonOptions;

    /// <summary>
    /// Create <see cref="TooStringOptions"/> from <paramref name="jsonSerializerOptions"/>
    /// </summary>
    public static implicit operator TooStringOptions(JsonSerializerOptions jsonSerializerOptions)
        => new() { JsonOptions = jsonSerializerOptions, StringifyAs = StringifyAs.STJsonSerialization };
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
